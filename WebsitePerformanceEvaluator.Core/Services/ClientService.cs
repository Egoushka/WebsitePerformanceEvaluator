using HtmlAgilityPack;
using Serilog;
using WebsitePerformanceEvaluator.Core.Extensions;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ClientService(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url)
    {
        _logger.Information("Start getting links by crawling");
        var links = new HashSet<string> { url };
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });
        const int semaphoreCount = 10;
        var semaphoreSlim = new SemaphoreSlim(semaphoreCount);

        while (linksToVisit.Count > 0)
        {
            var tasks = GetCrawlingTasks(linksToVisit, visitedLinks, semaphoreSlim);
            var results = await Task.WhenAll(tasks);

            var newLinks = results.SelectMany(result => result).ApplyFilters(url).ToList();

            links.UnionWith(newLinks);
            foreach (var link in newLinks.Except(visitedLinks))
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private IEnumerable<Task<IEnumerable<string>>> GetCrawlingTasks(Queue<string> linksToVisit,
        ICollection<string> visitedLinks, SemaphoreSlim semaphoreSlim)
    {
        var tasks = new List<Task<IEnumerable<string>>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);
            semaphoreSlim.Wait();

            var task = Task<IEnumerable<string>>.Factory.StartNew(() =>
            {
                var newLinks = CrawlPageToFindLinks(link);

                semaphoreSlim.Release();

                return newLinks;
            });
            tasks.Add(task);
        }

        return tasks;
    }

    private IEnumerable<string> CrawlPageToFindLinks(string url)
    {
        var doc = GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link => link.Attributes["href"].Value);
    }

    private HtmlDocument GetDocument(string url)
    {
        var doc = new HtmlDocument();
        var httpClient = _httpClientFactory.CreateClient();

        using var response = httpClient.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return doc;
    }

    public int GetTimeResponse(string url)
    {
        var time = 0;
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            var timeAtStart = DateTime.Now;

            var result = httpClient.GetAsync(url).Result;
            var responseTime = result.Headers.TryGetValues("X-Response-Time", out var values)
                ? values.FirstOrDefault()
                : null;
            time = responseTime == null ? (DateTime.Now - timeAtStart).Milliseconds : int.Parse(responseTime);
        }
        catch (Exception)
        {
            _logger.Error("Error while getting response time");
        }

        return time;
    }
}
