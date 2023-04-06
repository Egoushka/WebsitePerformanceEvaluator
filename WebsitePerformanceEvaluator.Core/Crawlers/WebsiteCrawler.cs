using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class WebsiteCrawler
{
    private readonly HtmlParser _htmlParser;
    private readonly LinkFilter _linkFilter;

    public WebsiteCrawler(HtmlParser htmlParser, LinkFilter linkFilter)
    {
        _htmlParser = htmlParser;
        _linkFilter = linkFilter;
    }

    public async Task<IEnumerable<LinkPerformance>> FindLinks(string url)
    {
        var links = new HashSet<LinkPerformance>();
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });

        while (linksToVisit.Count > 0)
        {
            var newLinks = await CrawlQueue(linksToVisit, visitedLinks);
            var filteredLinks = NormalizeLinks(newLinks, url);

            links.UnionWith(filteredLinks);

            var linksToAddToQueue = filteredLinks.SelectMany(item => item.FoundLinks)
                .Except(visitedLinks);
            
            foreach (var link in linksToAddToQueue)
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private async Task<IEnumerable<LinkPerformance>> CrawlQueue(Queue<string> linksToVisit,
        ICollection<string> visitedLinks)
    {
        var tasks = new List<Task<LinkPerformance>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<LinkPerformance>.Factory.StartNew(() =>
            {
                var newLinks = _htmlParser.GetLinks(link).Result;

                return newLinks;
            });
            
            tasks.Add(task);
        }
        var results = await Task.WhenAll(tasks);
        
        return results;
    }

    private IEnumerable<LinkPerformance> NormalizeLinks(IEnumerable<LinkPerformance> links, string url)
    {
        foreach (var result in links)
        {
            result.Link = result.Link.RemoveLastSlashFromLink();
            
            result.FoundLinks = result.FoundLinks.AddBaseUrl(url);
            
            result.FoundLinks = _linkFilter
                .FilterLinks(result.FoundLinks, url)
                .RemoveLastSlashFromLinks();
        }
        return links;
    }
}