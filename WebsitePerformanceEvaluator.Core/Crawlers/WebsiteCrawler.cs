using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Parsers;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class WebsiteCrawler
{
    private HtmlParser HtmlParser { get; }
    private LinkFilter LinkFilter { get; }

    public WebsiteCrawler(HtmlParser htmlParser, LinkFilter linkFilter)
    {
        HtmlParser = htmlParser;
        LinkFilter = linkFilter;
    }

    public async Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url)
    {
        var links = new HashSet<string> { url };
        var visitedLinks = new HashSet<string>();
        var linksToVisit = new Queue<string>(new[] { url });

        while (linksToVisit.Count > 0)
        {
            var tasks = GetCrawlingTasks(linksToVisit, visitedLinks);
            
            var filteredLinks = await GetFilteredLinksFromTasks(tasks, url);

            links.UnionWith(filteredLinks);
            foreach (var link in filteredLinks.Except(visitedLinks))
            {
                linksToVisit.Enqueue(link);
            }
        }

        return links;
    }

    private IEnumerable<Task<IEnumerable<string>>> GetCrawlingTasks(Queue<string> linksToVisit,
        ICollection<string> visitedLinks)
    {
        var tasks = new List<Task<IEnumerable<string>>>();

        for (var i = 0; i < linksToVisit.Count; i++)
        {
            var link = linksToVisit.Dequeue();

            visitedLinks.Add(link);

            var task = Task<IEnumerable<string>>.Factory.StartNew(() =>
            {
                var newLinks = HtmlParser.ParsePageToFindLinks(link);

                return newLinks;
            });
            tasks.Add(task);
        }

        return tasks;
    }
    private async Task<IEnumerable<string>> GetFilteredLinksFromTasks(IEnumerable<Task<IEnumerable<string>>> tasks, string url)
    {
        var results = await Task.WhenAll(tasks);

        var newLinks = results.SelectMany(result => result);
        var filteredLinks = LinkFilter.ApplyFilters(newLinks, url);
        
        return filteredLinks;
    }
}