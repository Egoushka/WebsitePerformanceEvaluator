using System.Xml;
using WebsitePerformanceEvaluator.Data.Enums;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Core.Parsers;

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
                Link = node["loc"].InnerText,
                CrawlingLinkSource = CrawlingLinkSource.Sitemap
            });
        }

        return result;
    }
}