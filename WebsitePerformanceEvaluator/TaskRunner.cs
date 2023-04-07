using System.Diagnostics;
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

    public async Task Start()
    {
        var watch = Stopwatch.StartNew();

        //var url = ConsoleHelper.GetInput("Enter url:");
        var url = "https://ukad-group.com/";

        watch.Start();

        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        PrintLinksInCrawlingNotInSitemap(links);
        PrintLinksInSitemapNotInCrawling(links);

        PrintLinksWithTimeResponse(links);

        PrintAmountOfFoundLinks(links);

        watch.Stop();
        Console.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }

    private void PrintLinksInCrawlingNotInSitemap(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInCrawlingNotInSitemap = linkPerformances
            .Where(x =>
                (x.CrawlingLinkType & CrawlingLinkType.Website) == CrawlingLinkType.Website
                &&
                (x.CrawlingLinkType & CrawlingLinkType.Sitemap) != CrawlingLinkType.Sitemap)
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

    private void PrintLinksInSitemapNotInCrawling(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInSitemapNotInCrawling = linkPerformances
            .Where(x =>
                (x.CrawlingLinkType & CrawlingLinkType.Website) != CrawlingLinkType.Website
                &&
                (x.CrawlingLinkType & CrawlingLinkType.Sitemap) == CrawlingLinkType.Sitemap)
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

    private void PrintLinksWithTimeResponse(IEnumerable<LinkPerformance> linkPerformances)
    {

        var rowsList = linkPerformances
            .OrderBy(item => item.TimeResponseMs)
            .Select(x => new Tuple<string, long>(x.Link, x.TimeResponseMs));

        Console.WriteLine("Links with time response:");

        ConsoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, rowsList);

        Console.WriteLine();
    }

    private void PrintAmountOfFoundLinks(IEnumerable<LinkPerformance> links)
    {
        var sitemapLinksCount = links.Count(l => (l.CrawlingLinkType & CrawlingLinkType.Sitemap) == CrawlingLinkType.Sitemap);

        var crawlingLinksCount = links.Count(l => (l.CrawlingLinkType & CrawlingLinkType.Website) == CrawlingLinkType.Website);

        Console.WriteLine($"Links in sitemap: {sitemapLinksCount}");
        Console.WriteLine($"Links after crawling: {crawlingLinksCount}");

        Console.WriteLine();
    }
}