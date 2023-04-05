using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
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

    public async Task<IEnumerable<string>> FindLinks(string url)
    {
        var links = new HashSet<string> { url };
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });

        while (linksToVisit.Count > 0)
        {
            var tasks = GetTasks(linksToVisit, visitedLinks);

            var filteredLinks = await GetLinksFromTasks(tasks, url);

            links.UnionWith(filteredLinks);
            foreach (var link in filteredLinks.Except(visitedLinks))
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private IEnumerable<Task<IEnumerable<string>>> GetTasks(Queue<string> linksToVisit,
        ICollection<string> visitedLinks)
    {
        var tasks = new List<Task<IEnumerable<string>>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<IEnumerable<string>>.Factory.StartNew(() =>
            {
                var newLinks = _htmlParser.GetLinks(link).Result;

                return newLinks;
            });
            tasks.Add(task);
        }

        return tasks;
    }

    private async Task<IEnumerable<string>> GetLinksFromTasks(IEnumerable<Task<IEnumerable<string>>> tasks,
        string url)
    {
        var results = await Task.WhenAll(tasks);

        var newLinks = results.SelectMany(result => result);
        var filteredLinks = _linkFilter.FilterLinks(newLinks, url)
            .RemoveLastSlashFromLinks()
            .AddBaseUrl(url);

        return filteredLinks;
    }
}