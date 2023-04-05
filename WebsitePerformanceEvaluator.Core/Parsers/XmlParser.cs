using System.Xml;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class XmlParser
{
    public IEnumerable<string> GetLinks(XmlNodeList xmlList)
    {
        var result = new List<string>();

        foreach (XmlNode node in xmlList)
        {
            if (node["loc"] != null)
            {
                result.Add(node["loc"]!.InnerText);
            }
        }

        return result;
    }
}