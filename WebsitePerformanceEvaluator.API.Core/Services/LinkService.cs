using System.ComponentModel.DataAnnotations;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.API.Core.Dto.Link;
using WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;
using WebsitePerformanceEvaluator.API.Core.Validators;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Models.Enums;
using WebsitePerformanceEvaluator.MVC.Core.ViewModels;
using LinkPerformance = WebsitePerformanceEvaluator.Core.Models.LinkPerformance;

namespace WebsitePerformanceEvaluator.API.Core.Services;

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

    public async Task<Result<GetLinkDto>> GetLinksAsync(int page, int pageSize)
    {
        var links = _context.Links.AsQueryable();
        
        var linksCount = await links.CountAsync();
        var totalPages = (int)Math.Ceiling(linksCount / (double)pageSize);

        var linksToShow = await links.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new GetLinkDto
        {
            Links = linksToShow.Select(item => new LinkDto
            {
                Id = item.Id,
                Url = item.Url
                
            }),
            CurrentPageIndex = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    public async Task<Result<CrawlLinkDto>> CrawlUrlAsync(string url)
    {
        var isLinkValid = _urlValidator.Validate(url);

        if (!isLinkValid)
        {
            var validationException = new ValidationException("Invalid url");

            return new Result<CrawlLinkDto>(validationException);
        }

        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        await SaveLinksToDatabaseAsync(links, url);
        
        return new CrawlLinkDto
        {
            Url = url,
            LinkPerformances = links.Select(item => new LinkPerformanceDto
            {
                Url = item.Url,
                TimeResponseMs = item.TimeResponseMs,
                CrawlingLinkSource = (int)item.CrawlingLinkSource,
            }).ToList(),
        };
    }

    private async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Link
        {
            Url = url,
        };

        var linksData = links.Select(x => new Data.Models.LinkPerformance
        {
            Url = x.Url,
            TimeResponseMs = x.TimeResponseMs,
            CrawlingLinkSource = (CrawlingLinkSource)x.CrawlingLinkSource,
            Link = link,
        });

        await _context.AddAsync(link);
        await _context.AddRangeAsync(linksData);
    }
}