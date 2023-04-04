using System.Xml;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class XmlParser
{
    private readonly HttpClientService _httpClientService;

    public XmlParser(HttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public IEnumerable<string> GetRawUrlsFromSitemap(XmlNodeList xmlSitemapList)
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