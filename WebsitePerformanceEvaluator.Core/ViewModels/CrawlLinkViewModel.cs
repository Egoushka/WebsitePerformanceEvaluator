
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Core.ViewModels;

public class CrawlLinkViewModel
{ 
    public string Url { get; set; }
    public IEnumerable<LinkPerformance> Urls { get; set; }
}