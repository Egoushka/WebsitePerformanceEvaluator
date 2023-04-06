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

    public async Task<LinkPerformance> GetLinks(string url)
    {
        var result = new LinkPerformance
        {
            Link = url,
            CrawlingLinkType = CrawlingLinkType.Website,
        };
       
        var doc = await _clientService.GetDocument(result);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes != null)
        {
            result.FoundLinks = linkNodes.Select(x => x.Attributes["href"].Value);
        }

        return result;
    }
}