namespace WebsitePerformanceEvaluator.API.Core.Dto.Link;

public class GetLinkDto
{
    public IEnumerable<LinkDto> Links { get; set; }
    public int CurrentPageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}