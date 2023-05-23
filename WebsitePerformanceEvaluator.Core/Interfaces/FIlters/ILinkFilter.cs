using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.FIlters;

public interface ILinkFilter
{
    public IEnumerable<LinkPerformance> FilterLinks(IEnumerable<LinkPerformance> links, string baseUrl);

}