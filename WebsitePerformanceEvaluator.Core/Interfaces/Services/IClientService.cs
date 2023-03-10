using System.Xml;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface IClientService
{
    public Task<IEnumerable<string>> CrawlPageToFindLinks(string url);

    public Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url);
    
    public int GetTimeResponse(string url);
}