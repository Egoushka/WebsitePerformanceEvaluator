using System.Xml;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class XmlParser
{
    public List<LinkPerformanceResult> GetLinks(XmlNodeList xmlList)
    {
        var result = new List<LinkPerformanceResult>();

        foreach (XmlNode node in xmlList)
        {
            if (node["loc"] == null) continue;
            
            var linkToAdd = new LinkPerformanceResult
            {
                Link = node["loc"]!.InnerText,
                CrawlingLinkType = CrawlingLinkType.Sitemap,
            };
            result.Add(linkToAdd);
        }

        return result;
    }
}