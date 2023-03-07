using System.Xml;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface IClientService
{
    public XmlDocument GetSitemap(string baseUrl);
    public IEnumerable<string> CrawlToFindLinks(string url);
    public int GetTimeResponse(string url);
}