using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Helpers;

public static class LinkHelper
{
    public static IEnumerable<LinkPerformance> AddBaseUrl(this IEnumerable<LinkPerformance> links, string baseUrl)
    {
        foreach (var link in links)
        {
            link.Link = link.Link.AddBaseUrl(baseUrl);
        }
        
        return links;
    }
    public static IEnumerable<LinkPerformance> RemoveLastSlashFromLinks(this IEnumerable<LinkPerformance> links)
    {
        foreach (var link in links)
        {
            link.Link = link.Link.RemoveLastSlashFromLink();
        }
        
        return links;
    }
    public static IEnumerable<string> AddBaseUrl(this IEnumerable<string> links, string baseUrl)
    {
        return links.Select(link => link.AddBaseUrl(baseUrl));
    }
    public static IEnumerable<string> RemoveLastSlashFromLinks(this IEnumerable<string> links)
    {
        return links.Select(link => link.RemoveLastSlashFromLink());
    }
    public static string AddBaseUrl(this string link, string baseUrl)
    {
        return link.StartsWith("/") ? baseUrl[..^1] + link : link;
    }
    public static string RemoveLastSlashFromLink(this string link)
    {
        return link.EndsWith("/") ? link.Remove(link.Length - 1) : link;
    }
}