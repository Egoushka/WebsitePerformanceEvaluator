using System.ComponentModel.DataAnnotations;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Interfaces.Validators;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.ViewModels;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Domain.Enums;

namespace WebsitePerformanceEvaluator.Core.Service;

public class LinkService : ILinkService
{
    private readonly ICrawler _crawler;
    private readonly ILinkValidator _urlValidator;
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;
    
    public LinkService(WebsitePerformanceEvaluatorDatabaseContext context, ICrawler crawler, ILinkValidator urlValidator)
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

        var links = await _crawler.FindLinksAsync(url);

        await SaveLinksToDatabaseAsync(links, url);

        return new CrawlLinkViewModel
        {
            Url = url,
            Urls = links,
        };
    }
    public async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Domain.Models.Link
        {
            Url = url,
        };

        var linksData = links.Select(x => new Domain.Models.LinkPerformance
        {
            Url = x.Url,
            TimeResponseMs = x.TimeResponseMs,
            CrawlingLinkSource = (CrawlingLinkSource)x.CrawlingLinkSource,
            Link = link,
        });

        await _context.AddAsync(link);
        await _context.AddRangeAsync(linksData);
        await _context.SaveChangesAsync();
    }
}