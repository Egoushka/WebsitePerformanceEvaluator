using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Crawlers;

public interface ICrawler
{ 
    Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url);
}