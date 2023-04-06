using System.Diagnostics;
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

    public async Task<LinkPerformanceResult> GetLinks(string url)
    {
        var result = new LinkPerformanceResult
        {
            Link = url,
            CrawlingLinkType = CrawlingLinkType.Website,
        };
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var doc = await _clientService.GetDocument(url);
        
        stopWatch.Stop();
        var time = stopWatch.ElapsedMilliseconds;
        result.TimeResponse = time;

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes != null)
        {
            result.FoundLinks = linkNodes.Select(x => x.Attributes["href"].Value).ToList();
        }

        return result;
    }
}