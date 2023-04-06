using System.Xml;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class XmlParser
{
    public IEnumerable<LinkPerformance> GetLinks(XmlNodeList xmlList)
    {
        var result = new List<LinkPerformance>();

        foreach (XmlNode node in xmlList)
        {
            if (node["loc"] == null) continue;
            
            var linkToAdd = new LinkPerformance
            {
                Link = node["loc"].InnerText,
                CrawlingLinkType = CrawlingLinkType.Sitemap,
                FoundLinks = new List<string>()
            };
            result.Add(linkToAdd);
        }

        return result;
    }
}