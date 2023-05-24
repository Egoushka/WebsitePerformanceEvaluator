
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Core.ViewModels;

public class LinkPerformanceViewModel
{
    public IEnumerable<LinkPerformance> LinkPerformances { get; set; }
    public Link Link { get; set; }
}