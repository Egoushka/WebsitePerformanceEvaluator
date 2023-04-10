using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class HtmlParser
{
    private readonly HttpClientService _clientService;

    public HtmlParser()
    {
    }

    public HtmlParser(HttpClientService httpClientService)
    {
        _clientService = httpClientService;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> GetLinksAsync(string url)
    {
        var result = new List<LinkPerformance>
        {
            new()
            {
                Link = url,
                CrawlingLinkSource = CrawlingLinkSource.Website
            }
        };

        var doc = await _clientService.GetDocumentAsync(result.First());

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes != null)
            result.AddRange(linkNodes.Select(linkNode => new LinkPerformance
            {
                Link = linkNode.Attributes["href"].Value,
                CrawlingLinkSource = CrawlingLinkSource.Website
            }));

        return result;
    }
}