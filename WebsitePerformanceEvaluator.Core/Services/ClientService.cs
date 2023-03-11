using System.Collections;
using System.Diagnostics;
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
        var visitedLinks = new List<string>();
        var linksToVisit = new List<string> { url };
        
        while (linksToVisit.Count > 0)
        {
            var tasks = GetCrawlingTasks(linksToVisit, links, visitedLinks, url);
            
            await Task.WhenAll(tasks);
        }

        return links;
    }
    private IEnumerable<Task> GetCrawlingTasks(List<string> linksToVisit, ISet<string> links, 
        ICollection<string> visitedLinks, string url)
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

            var task = Task.Run(async () =>
            {
                var newLinks = (await CrawlPageToFindLinks(link)).ApplyFilters(url).ToList();
                var filteredNewLinks = FilterNewLinks(newLinks, visitedLinks, linksToVisit);
                
                links.UnionWith(newLinks);
                linksToVisit.AddRange(filteredNewLinks);
                
                semaphoreSlim.Release();
                
                return Task.CompletedTask;
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
    public async Task<IEnumerable<string>> CrawlPageToFindLinks(string url)
    {
        var doc = await GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link =>
            link.Attributes["href"].Value);
    }

    private Task<HtmlDocument> GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = _httpClient.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return Task.FromResult(doc);
    }

    public int GetTimeResponse(string url)
    {
        var timeNow = DateTime.Now;
        
        try
        {
            _httpClient.GetAsync(url).Wait();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while getting response time");
        }
        
        var timeTaken = DateTime.Now - timeNow;

        return timeTaken.Milliseconds;
    }
}