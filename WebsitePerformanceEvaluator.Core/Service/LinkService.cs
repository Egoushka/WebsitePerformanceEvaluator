using System.ComponentModel.DataAnnotations;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Validators;
using WebsitePerformanceEvaluator.Domain.Models;
using WebsitePerformanceEvaluator.Domain.ViewModels;

namespace WebsitePerformanceEvaluator.Core.Service;

public class LinkService
{
    private readonly Crawler _crawler;
    private readonly LinkValidator _urlValidator;
    private readonly DatabaseContext _context;
    
    public LinkService(DatabaseContext context, Crawler crawler, LinkValidator urlValidator)
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
        var isLinkValid = _urlValidator.IsValidLink(url);

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