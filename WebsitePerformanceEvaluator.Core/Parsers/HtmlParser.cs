using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Parsers;

public class HtmlParser
{
    private HttpClientService ClientService { get; }

    public HtmlParser(HttpClientService httpClientService)
    {
        ClientService = httpClientService;
    }

    public async Task<IEnumerable<string>> GetLinks(string url)
    {
        var doc = await ClientService.GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link => link.Attributes["href"].Value);
    }
}