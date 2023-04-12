using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers.Common;

public static class CrawlersUtils
{
    public static IEnumerable<LinkPerformance> GetExpectedLinks(CrawlingLinkSource source, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}",
                TimeResponseMs = 100
            };
        }
    }
    public static IEnumerable<LinkPerformance> GetExpectedLinksWithoutTimeResponse(CrawlingLinkSource source, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}"
            };
        }
    }
}