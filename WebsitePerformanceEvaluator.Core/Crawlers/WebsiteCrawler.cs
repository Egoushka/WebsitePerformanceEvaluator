using System.Collections.Immutable;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class WebsiteCrawler
{
    private readonly HtmlParser _htmlParser;
    private readonly LinkFilter _linkFilter;
    private readonly LinkHelper _linkHelper;

    public WebsiteCrawler()
    {
    }

    public WebsiteCrawler(HtmlParser htmlParser, LinkFilter linkFilter, LinkHelper linkHelper)
    {
        _htmlParser = htmlParser;
        _linkFilter = linkFilter;
        _linkHelper = linkHelper;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url)
    {
        var links = new HashSet<LinkPerformance>();
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });
        
        var tasks = new List<Task<IEnumerable<LinkPerformance>>>();

        while (linksToVisit.Count > 0 || tasks.Any())
        {
            tasks.AddRange(CrawlQueueAsync(linksToVisit, visitedLinks));
            
            var newLinks = tasks.Where(item => item.IsCompleted).SelectMany(item => item.Result);
            var normalizedLinks = NormalizeLinks(newLinks, url);

            var linksWithResponseTime = normalizedLinks.Where(item => item.TimeResponseMs.HasValue);

            links.UnionWith(linksWithResponseTime);

            var linksToAddToQueue = normalizedLinks.Select(item => item.Url).Except(visitedLinks);

            foreach (var link in linksToAddToQueue)
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private IEnumerable<Task<IEnumerable<LinkPerformance>>> CrawlQueueAsync(Queue<string> linksToVisit,
        ICollection<string> visitedLinks)
    {
        var tasks = new List<Task<IEnumerable<LinkPerformance>>>();

        var semaphoreSlim = new SemaphoreSlim(50);

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            semaphoreSlim.Wait();

            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task.Run(async () => {
                var result = Enumerable.Empty<LinkPerformance>();
                try
                {
                    result = await _htmlParser.GetLinksAsync(link);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                semaphoreSlim.Release();

                return result;
            });

            tasks.Add(task);
        }

        return tasks;
    }

    private IEnumerable<LinkPerformance> NormalizeLinks(IEnumerable<LinkPerformance> links, string url)
    {
        links = links.Where(link => !string.IsNullOrEmpty(link.Url));

        links = _linkHelper.RemoveLastSlashFromLinks(links);

        links = _linkHelper.AddBaseUrl(links, url);

        links = _linkFilter.FilterLinks(links, url);

        return links;
    }
}