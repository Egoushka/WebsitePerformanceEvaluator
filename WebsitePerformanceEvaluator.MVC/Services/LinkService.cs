using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.MVC.ViewModels;
using LinkPerformance = WebsitePerformanceEvaluator.Core.Models.LinkPerformance;

namespace WebsitePerformanceEvaluator.MVC.Services;

public class LinkService
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;
    private readonly ILinkRepository _linkRepository;
    
    public LinkService(ILinkPerformanceRepository linkPerformanceRepository, ILinkRepository linkRepository)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
        _linkRepository = linkRepository;
    }
    public LinkViewModel GetLinks(int page, int pageSize)
    {
        var links = _linkRepository.GetAll()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var linksCount = links.Count();
        var totalPages = (int)Math.Ceiling(linksCount / (double)pageSize);

        var viewModel = new LinkViewModel
        {
            Links = links,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };

        return viewModel;
    }
    
    public async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Link
        {
            Url = url,
        };

        var linksData = links.Select(x => new WebsitePerformanceEvaluator.Data.Models.LinkPerformance
        {
            Url = x.Url,
            TimeResponseMs = x.TimeResponseMs,
            CrawlingLinkSource = (Data.Models.Enums.CrawlingLinkSource)x.CrawlingLinkSource,
            Link = link,
        });
        
        await _linkRepository.AddAsync(link);
        await _linkPerformanceRepository.AddRangeAsync(linksData);
    }
}