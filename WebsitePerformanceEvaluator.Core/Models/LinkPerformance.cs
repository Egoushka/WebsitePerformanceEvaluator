using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Models;

public class LinkPerformance
{
    public string Link { get; set; }
    public int TimeResponseMs { get; set; }
    public CrawlingLinkType CrawlingLinkType { get; set; }

    public override int GetHashCode()
    {
        return Link.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is LinkPerformance link && link.Link == Link;
    }
}