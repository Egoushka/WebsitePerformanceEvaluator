using WebsitePerformanceEvaluator.Data.Models.Enums;

namespace WebsitePerformanceEvaluator.Data.Models;

public class LinkPerformance
{
    public int Id { get; set; }
    public string Link { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }
}