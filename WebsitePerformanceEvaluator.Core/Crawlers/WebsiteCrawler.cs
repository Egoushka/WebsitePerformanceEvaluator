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

    public WebsiteCrawler(HtmlParser htmlParser, LinkFilter linkFilter, LinkHelper linkHelper)
    {
        _htmlParser = htmlParser;
        _linkFilter = linkFilter;
        _linkHelper = linkHelper;
    }

    public async Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url)
    {
        var links = new HashSet<LinkPerformance>();
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });

        while (linksToVisit.Count > 0)
        {
            var newLinks = await CrawlQueueAsync(linksToVisit, visitedLinks);
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

    private async Task<IEnumerable<LinkPerformance>> CrawlQueueAsync(Queue<string> linksToVisit,
        ISet<string> visitedLinks)
    {
        var tasks = new List<Task<IEnumerable<LinkPerformance>>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<IEnumerable<LinkPerformance>>.Factory.StartNew(() =>
            {
                var newLinks = _htmlParser.GetLinksAsync(link).Result;

                return newLinks;
            });
            
            tasks.Add(task);
        }
        var result = (await Task.WhenAll(tasks)).SelectMany(item => item);
        
        return result;
    }

    private IEnumerable<LinkPerformance> NormalizeLinks(IEnumerable<LinkPerformance> links, string url)
    {
        var linksWithTimeResponse = links.Where(item => item.TimeResponse > 0);
        links = linksWithTimeResponse.Concat(links.Except(linksWithTimeResponse));
        
        links = links.Where(link => !string.IsNullOrEmpty(link.Link));
            
        links = _linkHelper.RemoveLastSlashFromLinks(links);
        links = _linkHelper.AddBaseUrl(links, url);
        
        links = _linkFilter.FilterLinks(links, url);
        
        return links;
    }
}