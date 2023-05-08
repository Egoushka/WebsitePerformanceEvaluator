namespace WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;

public class LinkPerformanceDto
{
    public string Url { get; set; }
    public int? TimeResponseMs { get; set; }
    public int CrawlingLinkSource { get; set; }
}