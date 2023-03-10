using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class SitemapService : ISitemapService
{
    private IClientService ClientService { get; set; }

    public SitemapService(IClientService сlientService)
    {
        ClientService = сlientService;
    }

    public IEnumerable<string> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = ClientService.GetSitemap(baseUrl);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = GetRawUrlsFromSitemap(xmlSitemapList);

        return urls;
    }

    private IEnumerable<string> GetRawUrlsFromSitemap(XmlNodeList xmlSitemapList)
    {
        var result = new List<string>();

        foreach (XmlNode node in xmlSitemapList)
        {
            if (node["loc"] != null)
            {
                result.Add(node["loc"]!.InnerText);
            }
        }

        return result;
    }
}