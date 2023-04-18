using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.MVC.ViewModels;

public class LinkViewModel
{
    public IEnumerable<Link> Links { get; set; }
    //TODO CurrentPageIndex
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}