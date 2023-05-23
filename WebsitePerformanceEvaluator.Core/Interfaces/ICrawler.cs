using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces;

public interface ICrawler
{
    public Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url);
}