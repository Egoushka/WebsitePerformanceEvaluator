using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Helpers;

public interface ILinkHelper
{
    public IEnumerable<LinkPerformance> AddBaseUrl(IEnumerable<LinkPerformance> links, string baseUrl);

    public IEnumerable<LinkPerformance> RemoveLastSlashFromLinks(IEnumerable<LinkPerformance> links);

    public string AddBaseUrl(string link, string baseUrl);
    public string RemoveLastSlashFromLink(string link);

    public Task<IEnumerable<LinkPerformance>> AddResponseTimeAsync(IEnumerable<LinkPerformance> links);
}