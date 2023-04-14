using WebsitePerformanceEvaluator.Data.Models.Enums;

namespace WebsitePerformanceEvaluator.Data.Models;

public class LinkPerformance
{
    public int Id { get; set; }
    public int LinkId { get; set; }
    public string Link { get; set; }
    public int? TimeResponseMs { get; set; }
    public CrawlingLinkSource CrawlingLinkSource { get; set; }
    
    public virtual Link LinkNavigation { get; set; }
}