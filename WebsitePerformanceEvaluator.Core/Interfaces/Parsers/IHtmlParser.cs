using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Parsers;

public interface IHtmlParser
{
    public Task<IEnumerable<LinkPerformance>> GetLinksAsync(string url);
}