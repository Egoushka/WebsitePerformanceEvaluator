using System.Xml;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface IClientService
{
    public XmlDocument GetSitemap(string baseUrl);
    
    public IEnumerable<string> CrawlPageToFindLinks(string url);
    public IEnumerable<string> CrawlWebsiteToFindLinks(string url);
    
    public int GetTimeResponse(string url);
}