using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Models;

public class LinkPerformanceResult
{
    public string Link { get; set; }
    public long TimeResponse { get; set; }
    public CrawlingLinkType CrawlingLinkType { get; set; }
    public IEnumerable<string> FoundLinks { get; set; }
    
    public override int GetHashCode()
    {
        return Link.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is LinkPerformanceResult link && link.Link == Link;
    }
}