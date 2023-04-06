using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator;

public class TaskRunner
{
    private readonly Crawler _crawler;

    public TaskRunner(Crawler crawler)
    {
        _crawler = crawler;
    }

    public int LinksCountInSitemap { get; set; }
    public int LinksCountAfterCrawling { get; set; }
    public List<LinkPerformance> WebsiteCrawlingLinks { get; set; }
    public List<LinkPerformance> SitemapLinks { get; set; }

    public async Task Start()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        //var url = ConsoleHelper.GetInput("Enter url:");
        var url = "https://ukad-group.com/";

        watch.Start();

        await GetLinksFromWebsiteAnsSitemap(url);

        PrintLinksInCrawlingNotInSitemap();
        PrintLinksInSitemapNotInCrawling();
        PrintLinksWithTimeResponse();

        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");

        watch.Stop();
        Console.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }

    private async Task GetLinksFromWebsiteAnsSitemap(string url)
    {
        var links = await _crawler.CrawlWebsiteAndSitemap(url);

        WebsiteCrawlingLinks = GetLinksByType(links, CrawlingLinkType.Website).ToList();
        SitemapLinks = GetLinksByType(links, CrawlingLinkType.Sitemap).ToList();

        LinksCountInSitemap = SitemapLinks.Count;
        LinksCountAfterCrawling = WebsiteCrawlingLinks.Count;
    }

    private IEnumerable<LinkPerformance> GetLinksByType(IEnumerable<LinkPerformance> links,
        CrawlingLinkType type)
    {
        return links.Where(x => x.CrawlingLinkType == type);
    }

    private void PrintLinksInCrawlingNotInSitemap()
    {
        var linksInCrawlingNotInSitemap = WebsiteCrawlingLinks
            .Except(SitemapLinks)
            .Select(x => x.Link);
        
        Console.WriteLine("Links found after crawling website, but not in sitemap:");
        
        if (linksInCrawlingNotInSitemap.Any())
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInCrawlingNotInSitemap);
        }
        else
        {
            Console.WriteLine("No links found");
        }

        Console.WriteLine();
    }

    private void PrintLinksInSitemapNotInCrawling()
    {
        var linksInSitemapNotInCrawling = SitemapLinks
            .Except(WebsiteCrawlingLinks)
            .Select(x => x.Link);
        
        Console.WriteLine("Links in sitemap, that wasn't found after crawling:");

        if (linksInSitemapNotInCrawling.Any())
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInSitemapNotInCrawling);
        }
        else
        {
            Console.WriteLine("No links found");
        }

        Console.WriteLine();
    }

    private void PrintLinksWithTimeResponse()
    {
        var allLinks = WebsiteCrawlingLinks.Intersect(SitemapLinks);

        var rowsList = allLinks
            .Select(x => new Tuple<string, long>(x.Link, x.TimeResponse))
            .OrderBy(x => x.Item2);

        Console.WriteLine("Links with time response:");
        
        ConsoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, rowsList);
        
        Console.WriteLine();
    }
}