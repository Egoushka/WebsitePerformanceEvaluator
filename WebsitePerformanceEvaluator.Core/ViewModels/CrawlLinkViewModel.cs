
using WebsitePerformanceEvaluator.Domain.Models;
using LinkPerformance=WebsitePerformanceEvaluator.Core.Models.LinkPerformance;

namespace WebsitePerformanceEvaluator.Core.ViewModels;

public class CrawlLinkViewModel
{ 
    public string Url { get; set; }
    public IEnumerable<LinkPerformance> Urls { get; set; }
}