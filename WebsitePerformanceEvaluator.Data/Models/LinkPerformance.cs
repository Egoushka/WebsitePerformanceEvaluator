using WebsitePerformanceEvaluator.Data.Models.Enums;

namespace WebsitePerformanceEvaluator.Data.Models;

public class LinkPerformance
{
    public int Id { get; set; }
    public string Link { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }

    public override int GetHashCode()
    {
        return Link.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is LinkPerformance link && link.Link == Link;
    }
}