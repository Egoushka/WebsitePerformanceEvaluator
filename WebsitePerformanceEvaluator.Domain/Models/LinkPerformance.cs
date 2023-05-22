using WebsitePerformanceEvaluator.Domain.Enums;

namespace WebsitePerformanceEvaluator.Domain.Models;

public class LinkPerformance
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }
    public virtual Link Link { get; set; }
    public int LinkId { get; set; }
    
    public override int GetHashCode()
    {
        return Url.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is LinkPerformance link && link.Url == Url;
    }
}