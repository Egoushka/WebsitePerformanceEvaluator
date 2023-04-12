using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers.Common;

public static class CrawlersUtils
{
    public static IEnumerable<LinkPerformance> GetExpectedLinks(CrawlingLinkSource source, int count)
    {
        var links = new List<LinkPerformance>();
        for (var i = 0; i < count; i++)
        {
            links.Add(new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}",
                TimeResponseMs = 100
            });
        }

        return links;
    }
    public static IEnumerable<LinkPerformance> GetExpectedLinksWithoutTimeResponse(CrawlingLinkSource source, int count)
    {
        var response = new List<LinkPerformance>();
        for (var i = 0; i < count; i++)
        {
            response.Add(new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}"
            });
        }

        return response;
    }
}