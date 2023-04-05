using CommunityToolkit.HighPerformance.Buffers;
using WebsitePerformanceEvaluator.Core.Validators;

namespace WebsitePerformanceEvaluator.Core.Filters;

public class LinkFilter
{
    private LinkValidator Validator { get; }

    public LinkFilter()
    {
        Validator = new LinkValidator();
    }

    public IEnumerable<string> FilterLinks(IEnumerable<string> links, string baseUrl)
    {
        links = links.Distinct();
        
        links = RemoveInvalidLinks(links);
        links = RemoveExternalLinks(links, baseUrl);
        
        return links;
    }

    private IEnumerable<string> RemoveInvalidLinks(IEnumerable<string> links)
    {
        return links.Where(link => Validator.IsValidLink(link));
    }

    private IEnumerable<string> RemoveExternalLinks(IEnumerable<string> links, string baseUrl)
    {
        return links.Where(link => CompareHosts(link, baseUrl) != string.Empty);
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