using System.Collections;
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
            var normalizedLinks = NormalizeLinks(newLinks, url);

            links.UnionWith(normalizedLinks.Where(item => item.TimeResponse > 0));

            var linksToAddToQueue = normalizedLinks.Select(item => item.Link)
                .Except(visitedLinks);
            
            foreach (var link in linksToAddToQueue)
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private async Task<IEnumerable<LinkPerformance>> CrawlQueue(Queue<string> linksToVisit,
        ISet<string> visitedLinks)
    {
        var tasks = new List<Task<IEnumerable<LinkPerformance>>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<IEnumerable<LinkPerformance>>.Factory.StartNew(() =>
            {
                var newLinks = _htmlParser.GetLinks(link).Result;

                return newLinks;
            });
            
            tasks.Add(task);
        }
        var result = (await Task.WhenAll(tasks)).SelectMany(item => item);
        
        var linksWithTimeResponse = result.Where(item => item.TimeResponse > 0);
        result = linksWithTimeResponse.Concat(result.Except(linksWithTimeResponse));
        
        return result;
    }

    private IEnumerable<LinkPerformance> NormalizeLinks(IEnumerable<LinkPerformance> links, string url)
    {
        links = links
            .Where(link => !string.IsNullOrEmpty(link.Link))
            .RemoveLastSlashFromLinks()
            .AddBaseUrl(url);
        
        links = _linkFilter
            .FilterLinks(links, url);
        
        return links;
    }
}