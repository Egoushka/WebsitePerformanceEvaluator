using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class HtmlParser
{
    private readonly HttpClientService _clientService;
    
    public HtmlParser(HttpClientService httpClientService)
    {
        _clientService = httpClientService;
    }

    public async Task<IEnumerable<LinkPerformance>> GetLinksAsync(string url)
    {
        var result = new List<LinkPerformance>
        {
            new()
            {
                Link = url,
                CrawlingLinkType = CrawlingLinkType.Website,
            }
        };
        
        var doc = await _clientService.GetDocumentAsync(result.First());

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes != null)
        {
            result.AddRange(linkNodes.Select(linkNode => new LinkPerformance
            {
                Link = linkNode.Attributes["href"].Value,
                CrawlingLinkType = CrawlingLinkType.Website,
            }));
        }

        return result;
    }
}