using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Models.Enums;
using WebsitePerformanceEvaluator.MVC.Core.ViewModels;
using LinkPerformance = WebsitePerformanceEvaluator.Core.Models.LinkPerformance;

namespace WebsitePerformanceEvaluator.MVC.Core.Services;

public class LinkService
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;
    private readonly ILinkRepository _linkRepository;
    private readonly Crawler _crawler;
    
    public LinkService(ILinkPerformanceRepository linkPerformanceRepository, ILinkRepository linkRepository, Crawler crawler)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
        _linkRepository = linkRepository;
        _crawler = crawler;
    }

    public async Task<LinkViewModel> GetLinksAsync(int page, int pageSize)
    {
        var links = _linkRepository.GetAll();
        
        var linksCount = await links.CountAsync();
        var totalPages = (int)Math.Ceiling(linksCount / (double)pageSize);

        var linksToShow = await links.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new LinkViewModel
        {
            Links = linksToShow,
            CurrentPageIndex = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };

        return viewModel;
    }
    
    public async Task GetLinksFromUrlAsync(string url)
    {
        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        await SaveLinksToDatabaseAsync(links, url);
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
        
        await _linkRepository.AddAsync(link);
        await _linkPerformanceRepository.AddRangeAsync(linksData);
    }
}