using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Domain.ViewModels;

public class LinkViewModel
{
    public IEnumerable<Link> Links { get; set; }
    public int CurrentPageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}