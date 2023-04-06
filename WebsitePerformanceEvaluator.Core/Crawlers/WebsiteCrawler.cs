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

    public async Task<IEnumerable<LinkPerformanceResult>> FindLinks(string url)
    {
        var links = new HashSet<LinkPerformanceResult> { new(){ Link = url.RemoveLastSlashFromLink() } };
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });

        while (linksToVisit.Count > 0)
        {
            var tasks = GetTasks(linksToVisit, visitedLinks);

            var filteredLinks = await GetLinksFromTasks(tasks, url);

            links.UnionWith(filteredLinks);
            
            foreach (var link in filteredLinks.SelectMany(item=>item.FoundLinks).Except(visitedLinks))
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private IEnumerable<Task<LinkPerformanceResult>> GetTasks(Queue<string> linksToVisit,
        ICollection<string> visitedLinks)
    {
        var tasks = new List<Task<LinkPerformanceResult>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<LinkPerformanceResult>.Factory.StartNew(() =>
            {
                var newLinks = _htmlParser.GetLinks(link).Result;

                return newLinks;
            });
            tasks.Add(task);
        }

        return tasks;
    }

    private async Task<IEnumerable<LinkPerformanceResult>> GetLinksFromTasks(IEnumerable<Task<LinkPerformanceResult>> tasks,
        string url)
    {
        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            result.Link = result.Link.RemoveLastSlashFromLink();
            result.FoundLinks = result.FoundLinks.AddBaseUrl(url);
            result.FoundLinks = _linkFilter
                .FilterLinks(result.FoundLinks, url)
                .RemoveLastSlashFromLinks();
        }
        
        
        return results;
    }
}