namespace WebsitePerformanceEvaluator.Core.Helpers;

public static class LinkHelper
{
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