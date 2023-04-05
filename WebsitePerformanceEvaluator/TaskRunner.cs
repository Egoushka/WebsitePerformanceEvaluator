using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Data.Enums;

namespace WebsitePerformanceEvaluator;

public class TaskRunner
{
    private Crawler Crawler { get; }
    private int LinksCountInSitemap { get; set; }
    private int LinksCountAfterCrawling { get; set; }
    private List<string> WebsiteCrawlingLinks { get; set; }
    private List<string> SitemapLinks { get; set; }

    public TaskRunner(Crawler crawler)
    {
        Crawler = crawler;
    }

    public async Task Start()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        //var url = ConsoleHelper.GetInput("Enter url:");
        var url = "https://ukad-group.com/";
        
        watch.Start();
        
        await GetLinksFromWebsiteAnsSitemap(url);

        PrintLinksInCrawlingNotInSitemap();
        PrintLinksInSitemapNotInCrawling();
        
        await PrintLinksWithTimeResponse(url);


        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");
        
        watch.Stop();
        Console.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }
    private async Task GetLinksFromWebsiteAnsSitemap(string url)
    {
        var links = await Crawler.GetLinksByCrawlingAndSitemap(url);
        
        WebsiteCrawlingLinks = GetLinksByType(links, CrawlingLinkType.Website).ToList();
        SitemapLinks = GetLinksByType(links, CrawlingLinkType.Sitemap).ToList();
        
        LinksCountInSitemap = WebsiteCrawlingLinks.Count;
        LinksCountAfterCrawling = SitemapLinks.Count;
    }
    private IEnumerable<string> GetLinksByType(IEnumerable<Tuple<string, CrawlingLinkType>> links, CrawlingLinkType type)
    {
        return links.Where(x => x.Item2 == type).Select(x => x.Item1);
    }

    private void PrintLinksInCrawlingNotInSitemap()
    {
        var linksInCrawlingNotInSitemap = WebsiteCrawlingLinks.Except(SitemapLinks).ToList();
        Console.WriteLine("Links found after crawling, but not in sitemap:");
        if (!linksInCrawlingNotInSitemap.Any())
        {
            Console.WriteLine("No links found");
        }
        else
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInCrawlingNotInSitemap);
        }

        Console.WriteLine();
    }

    private void PrintLinksInSitemapNotInCrawling()
    {
        var linksInSitemapNotInCrawling = SitemapLinks.Except(WebsiteCrawlingLinks).ToList();
        Console.WriteLine("Links in sitemap, that wasn't found after crawling:");

        if (!linksInSitemapNotInCrawling.Any())
        {
            Console.WriteLine("No links found");
        }
        else
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInSitemapNotInCrawling);
        }

        Console.WriteLine();
    }

    private async Task PrintLinksWithTimeResponse(string url)
    {
        var linksWithTimeResponse = (await Crawler.GetLinksWithTimeResponse(url))
            .OrderBy(item => item.Item2)
            .ToList();
        Console.WriteLine("Links with time response:");

        ConsoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, linksWithTimeResponse);
        Console.WriteLine();
    }
}