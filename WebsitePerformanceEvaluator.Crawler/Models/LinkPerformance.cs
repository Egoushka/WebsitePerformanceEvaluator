using WebsitePerformanceEvaluator.Crawler.Models.Enums;

namespace WebsitePerformanceEvaluator.Crawler.Models;

public class LinkPerformance
{
    public string Url { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }
    public override int GetHashCode()
    {
        return Url.GetHashCode();
    }
    
    public override bool Equals(object? obj)
    {
        return obj is LinkPerformance link && link.Url == Url;
    }
}