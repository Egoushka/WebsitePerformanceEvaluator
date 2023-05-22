using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Console.Services;

public class LinkService
{
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;
    public LinkService(WebsitePerformanceEvaluatorDatabaseContext context)
    {
        _context = context;
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