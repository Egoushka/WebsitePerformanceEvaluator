using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class HtmlParser
{
    private readonly HttpClientService _httpClientService;

    public HtmlParser(HttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public IEnumerable<string> ParsePageToFindLinks(string url)
    {
        var doc = _httpClientService.GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link => link.Attributes["href"].Value);
    }
}