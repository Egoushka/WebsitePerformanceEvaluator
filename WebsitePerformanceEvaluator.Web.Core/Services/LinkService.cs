using System.ComponentModel.DataAnnotations;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Domain.Models;
using WebsitePerformanceEvaluator.Web.Core.Validators;
using WebsitePerformanceEvaluator.Web.Core.ViewModels;

namespace WebsitePerformanceEvaluator.Web.Core.Services;

public class LinkService
{
    private readonly Crawler _crawler;
    private readonly UrlValidator _urlValidator;
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;
    
    public LinkService(WebsitePerformanceEvaluatorDatabaseContext context, Crawler crawler, UrlValidator urlValidator)
    {
        _context = context;
        _crawler = crawler;
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
        var isLinkValid = _urlValidator.Validate(url);

        if (!isLinkValid)
        {
            var validationException = new ValidationException("Invalid url");

            return new Result<CrawlLinkViewModel>(validationException);
        }

        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        await SaveLinksToDatabaseAsync(links, url);

        return new CrawlLinkViewModel
        {
            Url = url,
            Urls = links,
        };
    }

    private async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
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