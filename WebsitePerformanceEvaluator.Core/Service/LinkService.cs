using System.ComponentModel.DataAnnotations;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.ViewModels;
using WebsitePerformanceEvaluator.Crawler.Validators;
using WebsitePerformanceEvaluator.Crawler.Crawlers;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Domain.Enums;
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Core.Service;

public class LinkService
{
    private readonly CombinedCrawler _combinedCrawler;
    private readonly LinkValidator _urlValidator;
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;
    
    public LinkService(WebsitePerformanceEvaluatorDatabaseContext context, Crawler.Crawlers.CombinedCrawler combinedCrawler, LinkValidator urlValidator)
    {
        _context = context;
        _combinedCrawler = combinedCrawler;
        _urlValidator = urlValidator;
    }

    public async Task<Result<LinkViewModel>> GetLinksAsync(int page, int pageSize)
    {
        var links = _context.Links.AsQueryable();
        
        var linksCount = await links.CountAsync();
        var totalPages = (int)Math.Ceiling(linksCount / (double)pageSize);

        var linksToShow = await links.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new LinkViewModel
        {
            Links = linksToShow,
            CurrentPageIndex = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    public async Task<Result<CrawlLinkViewModel>> CrawlUrlAsync(string url)
    {
        var isLinkValid = _urlValidator.IsValidLink(url);

        if (!isLinkValid)
        {
            var validationException = new ValidationException("Invalid url");

            return new Result<CrawlLinkViewModel>(validationException);
        }

        var links =
            (await _combinedCrawler.CrawlWebsiteAndSitemapAsync(url))
            .Select(x => new LinkPerformance
            {
                Url = x.Url,
                TimeResponseMs = x.TimeResponseMs,
                CrawlingLinkSource = (CrawlingLinkSource)x.CrawlingLinkSource,
            });

        await SaveLinksToDatabaseAsync(links, url);

        return new CrawlLinkViewModel
        {
            Url = url,
            Urls = links,
        };
    }

    public async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Link
        {
            Url = url,
        };

        var linksData = links.Select(x => new LinkPerformance
        {
            Url = x.Url,
            TimeResponseMs = x.TimeResponseMs,
            CrawlingLinkSource = x.CrawlingLinkSource,
            Link = link,
        });

        await _context.AddAsync(link);
        await _context.AddRangeAsync(linksData);
        await _context.SaveChangesAsync();
    }
}