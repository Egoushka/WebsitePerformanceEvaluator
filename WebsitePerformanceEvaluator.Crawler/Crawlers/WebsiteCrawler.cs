using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Crawler.Filters;
using WebsitePerformanceEvaluator.Crawler.Helpers;
using WebsitePerformanceEvaluator.Crawler.Parsers;

namespace WebsitePerformanceEvaluator.Crawler.Crawlers;

public class WebsiteCrawler : ICrawler
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
        
        while (linksToVisit.Count > 0)
        {
            var newLinks = await CrawlQueueAsync(linksToVisit, visitedLinks);

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

    private async Task<IEnumerable<LinkPerformance>> CrawlQueueAsync(Queue<string> linksToVisit,
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
        var result = (await Task.WhenAll(tasks)).SelectMany(item => item);
        
        return result;
    }

    private IEnumerable<LinkPerformance> NormalizeLinks(IEnumerable<LinkPerformance> links, string url)
    {
        try
        {
            links = links.Where(link => !string.IsNullOrEmpty(link.Url));

            links = _linkHelper.RemoveLastSlashFromLinks(links);

            links = _linkHelper.AddBaseUrl(links, url);

            links = _linkFilter.FilterLinks(links, url);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
 

        return links;
    }
}