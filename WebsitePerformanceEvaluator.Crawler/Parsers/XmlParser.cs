using System.Xml;
using WebsitePerformanceEvaluator.Crawler.Models;
using WebsitePerformanceEvaluator.Crawler.Models.Enums;

namespace WebsitePerformanceEvaluator.Crawler.Parsers;

public class XmlParser
{
    public virtual IEnumerable<LinkPerformance> GetLinks(XmlNodeList xmlList)
    {
        var result = new List<LinkPerformance>();

        foreach (XmlNode node in xmlList)
        {
            if (node["loc"] == null) continue;

            result.Add(new LinkPerformance
            {
                Url = node["loc"].InnerText,
                CrawlingLinkSource = CrawlingLinkSource.Sitemap
            });
        }

        return result;
    }
}