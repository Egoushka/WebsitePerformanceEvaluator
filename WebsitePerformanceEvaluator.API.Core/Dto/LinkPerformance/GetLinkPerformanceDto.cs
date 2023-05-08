namespace WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;

public class GetLinkPerformanceDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int? TimeResponseMs { get; set; }
    public int CrawlingLinkSource { get; set; }
}