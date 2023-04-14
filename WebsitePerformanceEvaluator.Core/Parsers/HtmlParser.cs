using HtmlAgilityPack;
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
                Url = url,
                CrawlingLinkSource = CrawlingLinkSource.Website
            }
        };
        var doc = new HtmlDocument();
        
        try
        {
            doc = await _clientService.GetDocumentAsync(result.First());
        }
        catch (ArgumentException)
        {
            return new List<LinkPerformance>();
        }
        
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes != null)
        {
            var links = linkNodes.Select(linkNode => new LinkPerformance
            {
                Url = linkNode.Attributes["href"].Value,
                CrawlingLinkSource = CrawlingLinkSource.Website
            });
            result.AddRange(links);

        }

        return result;
    }
}