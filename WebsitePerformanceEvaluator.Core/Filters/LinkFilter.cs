using CommunityToolkit.HighPerformance.Buffers;

namespace WebsitePerformanceEvaluator.Core.Filters;

public class LinkFilter
{
    private readonly Func<string, bool>[] _predicates;

    public LinkFilter()
    {
        _predicates = new[]
        {
            DoesNotContainAnchor,
            IsNotFileLink,
            DoesNotContainAttributes,
        };
    }

    public IEnumerable<string> FilterLinks(IEnumerable<string> links, string baseUrl)
    {
        links = links.Distinct();

        links = _predicates.Aggregate(links, (current, predicate) => current.Where(predicate));

        links = AddBaseUrl(links, baseUrl);
        links = CheckForSlashAndRemove(links);
        links = CheckLinksHosts(links, baseUrl);

        return links;
    }
    private bool DoesNotContainAnchor(string link)
    {
        return !link.Contains('#');
    }

    private bool IsNotFileLink(string link)
    {
        return link.LastIndexOf('.') < link.LastIndexOf('/');
    }

    private bool DoesNotContainAttributes(string link)
    {
        return !link.Contains('?');
    }

    private bool IsHttpOrHttpsLink(string link)
    {
        return link.StartsWith(Uri.UriSchemeHttp) || link.StartsWith(Uri.UriSchemeHttps);
    }
    private IEnumerable<string> AddBaseUrl(IEnumerable<string> links, string baseUrl)
    {
        return links.Select(link => link.StartsWith("/") ? baseUrl[..^1] + link : link);
    }

    private IEnumerable<string> CheckForSlashAndRemove(IEnumerable<string> links)
    {
        return links.Select(link => link.EndsWith("/") ? link.Remove(link.Length - 1) : link);
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