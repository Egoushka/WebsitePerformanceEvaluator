using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Helpers;

public class LinkHelper
{
    public IEnumerable<LinkPerformance> AddBaseUrl(IEnumerable<LinkPerformance> links, string baseUrl)
    {
        foreach (var link in links)
        {
            link.Link = AddBaseUrl(link.Link, baseUrl);
        }
        
        return links;
    }
    public IEnumerable<LinkPerformance> RemoveLastSlashFromLinks(IEnumerable<LinkPerformance> links)
    {
        foreach (var link in links)
        {
            link.Link = RemoveLastSlashFromLink(link.Link);
        }
        
        return links;
    }
    public string AddBaseUrl(string link, string baseUrl)
    {
        return link.StartsWith("/") ? baseUrl[..^1] + link : link;
    }
    public string RemoveLastSlashFromLink(string link)
    {
        return link.EndsWith("/") ? link.Remove(link.Length - 1) : link;
    }
}