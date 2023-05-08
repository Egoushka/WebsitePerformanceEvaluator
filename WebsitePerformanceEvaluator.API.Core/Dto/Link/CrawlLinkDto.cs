using WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;

namespace WebsitePerformanceEvaluator.API.Core.Dto.Link;

public class CrawlLinkDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public IEnumerable<LinkPerformanceDto> LinkPerformances { get; set; }
}