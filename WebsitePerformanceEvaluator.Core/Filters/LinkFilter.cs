using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Validators;

namespace WebsitePerformanceEvaluator.Core.Filters;

public class LinkFilter
{
    private readonly LinkValidator _validator;
    public LinkFilter()
    {
        _validator = new LinkValidator();
    }

    public IEnumerable<LinkPerformance> FilterLinks(IEnumerable<LinkPerformance> links, string baseUrl)
    {
        return FilterLinks(links.Select(link => link.Link), baseUrl)
            .Select(filteredLink => links.First(l => l.Link == filteredLink));
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
        return links.Where(link => _validator.IsValidLink(link));
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
        var host = new Uri(url).Host;

        return host;
    }
}