using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Models;

public class LinkPerformance
{
    public int Id { get; set; }
    public string Link { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }
}