using HtmlAgilityPack;
using Serilog;
using WebsitePerformanceEvaluator.Core.Extensions;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class WebsiteCrawler
{
    private readonly HttpClientService _httpClientService;

    public WebsiteCrawler(HttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url)
    {
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
        var doc = _httpClientService.GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link => link.Attributes["href"].Value);
    }

 
}