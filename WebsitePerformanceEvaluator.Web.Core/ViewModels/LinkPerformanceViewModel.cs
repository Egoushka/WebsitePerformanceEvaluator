using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Web.Core.ViewModels;

public class LinkPerformanceViewModel
{
    public IEnumerable<LinkPerformance> LinkPerformances { get; set; }
    public Link Link { get; set; }
}