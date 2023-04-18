using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
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
    public IEnumerable<Link> GetLinks()
    {
        return _linkRepository.GetAll();
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