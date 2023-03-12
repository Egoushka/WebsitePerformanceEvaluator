using HtmlAgilityPack;
using Serilog;
using WebsitePerformanceEvaluator.Core.Extensions;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService : IClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public ClientService(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url)
    {
        _logger.Information("Start getting links by crawling");
        var links = new HashSet<string> { url };
        var visitedLinks = new SynchronizedCollection<string>();
        var linksToVisit = new SynchronizedCollection<string> { url };

        while (linksToVisit.Count > 0)
        {
            var tasks = GetCrawlingTasks(links, linksToVisit, visitedLinks, url);

            await Task.WhenAll(tasks);
        }

        return links;
    }

    private IEnumerable<Task> GetCrawlingTasks(ISet<string> links, SynchronizedCollection<string> linksToVisit,
        SynchronizedCollection<string> visitedLinks, string url)
    {
        const int semaphoreCount = 8;

        var tasks = new List<Task>();
        var semaphoreSlim = new SemaphoreSlim(semaphoreCount);

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit[i];

            linksToVisit.RemoveAt(i);

            visitedLinks.Add(link);
            semaphoreSlim.Wait();

            var task = Task.Run(() =>
            {
                var newLinks = CrawlPageToFindLinks(link).ApplyFilters(url).ToList();
                var filteredNewLinks = FilterNewLinks(newLinks, visitedLinks, linksToVisit);

                lock (links)
                {
                    links.UnionWith(newLinks);
                }

                foreach (var item in filteredNewLinks.Where(item =>
                             !linksToVisit.Contains(item) && !visitedLinks.Contains(item)))
                {
                    linksToVisit.Add(item);
                }

                semaphoreSlim.Release();

                return Task.FromResult(Task.CompletedTask);
            });
            tasks.Add(task);
        }

        return tasks;
    }

    private IEnumerable<string> FilterNewLinks(IEnumerable<string> newLinks, ICollection<string> visitedLinks,
        ICollection<string> linksToVisit)
    {
        return newLinks.Where(newLink =>
            !visitedLinks.Contains(newLink) && !linksToVisit.Contains(newLink));
    }

    public IEnumerable<string> CrawlPageToFindLinks(string url)
    {
        var doc = GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link =>
            link.Attributes["href"].Value);
    }

    private HtmlDocument GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = _httpClient.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return doc;
    }

    public int GetTimeResponse(string url)
    {
        var time = 0;
        try
        {
            var timeAtStart = DateTime.Now;

            var result = _httpClient.GetAsync(url).Result;
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