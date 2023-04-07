using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Validators;

namespace WebsitePerformanceEvaluator.Core.Filters;

public class LinkFilter
{
    private readonly LinkValidator _validator;

    public LinkFilter(LinkValidator validator)
    {
        _validator = validator;
    }

    public virtual IEnumerable<LinkPerformance> FilterLinks(IEnumerable<LinkPerformance> links, string baseUrl)
    {
        links = links.Distinct();
        links = RemoveInvalidLinks(links);
        links = RemoveUnsupportedSchemes(links);
        links = RemoveExternalLinks(links, baseUrl);

        return links;
    }

    private IEnumerable<LinkPerformance> RemoveInvalidLinks(IEnumerable<LinkPerformance> links)
    {
        return links.Where(link => _validator.IsValidLink(link.Link));
    }

    private IEnumerable<LinkPerformance> RemoveExternalLinks(IEnumerable<LinkPerformance> links, string baseUrl)
    {
        return links.Where(link => CompareHosts(link.Link, baseUrl) != string.Empty);
    }

    private IEnumerable<LinkPerformance> RemoveUnsupportedSchemes(IEnumerable<LinkPerformance> links)
    {
        return links.Where(link => link.Link.StartsWith(Uri.UriSchemeHttp) || link.Link.StartsWith(Uri.UriSchemeHttps));
    }

    private string CompareHosts(string url, string baseUrl)
    {
        var urlHost = GetHost(url);
        var baseUrlHost = GetHost(baseUrl);

        return urlHost == baseUrlHost ? url : string.Empty;
    }

    private string GetHost(string url)
    {
        var host = new Uri(url).Host;

        return host;
    }
}