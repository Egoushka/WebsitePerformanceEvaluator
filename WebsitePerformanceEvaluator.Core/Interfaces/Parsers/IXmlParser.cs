using System.Xml;
using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Parsers;

public interface IXmlParser
{
    public IEnumerable<LinkPerformance> GetLinks(XmlNodeList xmlList);
}