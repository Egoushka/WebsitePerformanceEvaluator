using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.MVC.Core.ViewModels;

public class LinkPerformanceViewModel
{
    public IEnumerable<LinkPerformance> LinkPerformances { get; set; }
    public Link Link { get; set; }
}