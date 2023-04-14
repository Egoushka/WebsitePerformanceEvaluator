using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

namespace WebsitePerformanceEvaluator.Console.Services;

public class LinkService
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;
    private readonly ILinkRepository _linkRepository;
    
    public LinkService(ILinkPerformanceRepository linkPerformanceRepository, ILinkRepository linkRepository)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
        _linkRepository = linkRepository;
    }

    public async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Data.Models.Link
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