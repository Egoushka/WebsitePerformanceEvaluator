using CommunityToolkit.HighPerformance.Buffers;

namespace WebsitePerformanceEvaluator.Core.Filters;

public class LinkFilter
{
    public IEnumerable<string> ApplyFilters(IEnumerable<string> links, string baseUrl)
    {
        links = links.Distinct();

        links = RemoveAnchorLinks(links);
        links = RemoveFilesLinks(links);
        links = RemoveLinksWithAttributes(links);
        links = AddBaseUrl(links, baseUrl);
        links = CheckForSlashAndRemove(links);
        links = CheckLinksHosts(links, baseUrl);
        links = RemoveNotHttpsOrHttpScheme(links);

        return links;
    }

    private IEnumerable<string> AddBaseUrl(IEnumerable<string> links, string baseUrl)
    {
        return links.Select(link => link.StartsWith("/") ? baseUrl[..^1] + link : link);
    }

    private IEnumerable<string> CheckForSlashAndRemove(IEnumerable<string> links)
    {
        return links.Select(link => link.EndsWith("/") ? link.Remove(link.Length - 1) : link);
    }

    private IEnumerable<string> RemoveAnchorLinks(IEnumerable<string> links)
    {
        return links.Where(link => !link.Contains('#'));
    }

    private IEnumerable<string> RemoveFilesLinks(IEnumerable<string> links)
    {
        return links.Where(link => link.LastIndexOf('.') < link.LastIndexOf('/'));
    }

    private IEnumerable<string> RemoveLinksWithAttributes(IEnumerable<string> links)
    {
        return links.Where(link => !link.Contains('?'));
    }

    private IEnumerable<string> RemoveNotHttpsOrHttpScheme(IEnumerable<string> links)
    {
        return links.Where(link => link.StartsWith(Uri.UriSchemeHttp) || link.StartsWith(Uri.UriSchemeHttps));
    }

    private IEnumerable<string> CheckLinksHosts(IEnumerable<string> urls, string baseUrl)
    {
        return urls.Where(url => CompareHosts(url, baseUrl) != string.Empty);
    }

    private string CompareHosts(string url, string baseUrl)
    {
        var urlHost = GetHost(url);
        var baseUrlHost = GetHost(baseUrl);
        return urlHost == baseUrlHost ? url : string.Empty;
    }

    private string GetHost(string url)
    {
        const int schemeLength = 3;
        var prefixOffset = url.AsSpan().IndexOf(stackalloc char[] { ':', '/', '/' });
        var startIndex = prefixOffset == -1 ? 0 : prefixOffset + schemeLength;
        var endIndex = url.AsSpan(startIndex).IndexOf('/');

        var host = endIndex == -1 ? url.AsSpan(startIndex) : url.AsSpan(startIndex, endIndex);
        return StringPool.Shared.GetOrAdd(host);
    }
}