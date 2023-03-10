using CommunityToolkit.HighPerformance.Buffers;

namespace WebsitePerformanceEvaluator.Core.Extensions;

public static class LinkFilterExtension
{
    public static IEnumerable<string> ApplyFilters(this IEnumerable<string> links, string baseUrl)
    {
        return links
            .Distinct()
            .RemoveAnchorLinks()
            .RemoveFilesLinks()
            .RemoveLinksWithAttributes()
            .AddBaseUrl(baseUrl)
            .CheckForSlashAndRemove()
            .CheckLinksHosts(baseUrl)
            .RemoveNotHttpsOrHttpScheme();

    }
    private static IEnumerable<string> AddBaseUrl(this IEnumerable<string> links, string baseUrl)
    {
        return links.Select(link => link.StartsWith("/") ? baseUrl[..^1] + link : link);
    }
    private static IEnumerable<string> CheckForSlashAndRemove(this IEnumerable<string> links)
    {
        return links.Select(link => link.EndsWith("/") ? link.Remove(link.Length - 1) : link);
    }
    private static IEnumerable<string> RemoveAnchorLinks(this IEnumerable<string> links)
    {
        return links.Where(link => !link.Contains('#'));
    }
    private static IEnumerable<string> RemoveFilesLinks(this IEnumerable<string> links)
    {
        return links.Where(link => !link.Contains('.') || link.LastIndexOf('.') < link.LastIndexOf('/'));
    }
    private static IEnumerable<string> RemoveNotHttpsOrHttpScheme(this IEnumerable<string> links)
    {
        return links.Where(link => link.StartsWith(Uri.UriSchemeHttp) || link.StartsWith(Uri.UriSchemeHttps));
    }
    private static IEnumerable<string> CheckLinksHosts(this IEnumerable<string> urls, string baseUrl)
    {
        return urls.Where(url => url.CompareHosts(baseUrl) != string.Empty);
    }
    private static string CompareHosts(this string url, string baseUrl)
    {
        var urlHost = url.GetHost();
        var baseUrlHost = baseUrl.GetHost();
        return urlHost == baseUrlHost ? url : string.Empty;
    }
    private static string GetHost(this string url)
    {
        const int schemeLength = 3;
        var prefixOffset = url.AsSpan().IndexOf(stackalloc char[]{':', '/', '/'});
        var startIndex = prefixOffset == -1 ? 0 : prefixOffset + schemeLength;
        var endIndex = url.AsSpan(startIndex).IndexOf('/');
        
        var host = endIndex == -1 ? url.AsSpan(startIndex) : 
            url.AsSpan(startIndex, endIndex);
        return StringPool.Shared.GetOrAdd(host);
    }
}