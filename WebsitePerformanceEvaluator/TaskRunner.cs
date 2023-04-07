using System.Diagnostics;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator;

public class TaskRunner
{
    private readonly Crawler _crawler;
    private readonly ConsoleWrapper _consoleWrapper;

    public TaskRunner(Crawler crawler, ConsoleWrapper consoleWrapper)
    {
        _crawler = crawler;
        _consoleWrapper = consoleWrapper;
    }
    public async Task Run()
    {
        var watch = Stopwatch.StartNew();

        //_consoleWrapper.WriteLine("Enter website url:");
        //var url = _consoleWrapper.ReadLine();
        var url = "https://ukad-group.com/";

        watch.Start();

        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        PrintLinksInCrawlingNotInSitemap(links);
        PrintLinksInSitemapNotInCrawling(links);

        PrintLinksWithTimeResponse(links);

        PrintAmountOfFoundLinks(links);

        watch.Stop();
        _consoleWrapper.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }

    private void PrintLinksInCrawlingNotInSitemap(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInCrawlingNotInSitemap = linkPerformances
            .Where(x => x.CrawlingLinkSource == CrawlingLinkSource.Website)
            .Select(x => x.Link);

        _consoleWrapper.WriteLine("Links found after crawling website, but not in sitemap:");

        if (linksInCrawlingNotInSitemap.Any())
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInCrawlingNotInSitemap);
        }
        else
        {
            _consoleWrapper.WriteLine("No links found");
        }

        _consoleWrapper.WriteLine();
    }

    private void PrintLinksInSitemapNotInCrawling(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInSitemapNotInCrawling = linkPerformances
                .Where(link => link.CrawlingLinkSource == CrawlingLinkSource.Sitemap)
                .Select(link => link.Link);

        _consoleWrapper.WriteLine("Links in sitemap, that wasn't found after crawling:");

        if (linksInSitemapNotInCrawling.Any())
        {
            ConsoleHelper.PrintTable(new List<string> { "Link" }, linksInSitemapNotInCrawling);
        }
        else
        {
            _consoleWrapper.WriteLine("No links found");
        }

        _consoleWrapper.WriteLine();
    }

    private void PrintLinksWithTimeResponse(IEnumerable<LinkPerformance> linkPerformances)
    {

        var rowsList = linkPerformances
            .OrderBy(item => item.TimeResponseMs)
            .Select(x => new Tuple<string, long?>(x.Link, x.TimeResponseMs));

        _consoleWrapper.WriteLine("Links with time response:");

        ConsoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, rowsList);

        _consoleWrapper.WriteLine();
    }

    private void PrintAmountOfFoundLinks(IEnumerable<LinkPerformance> links)
    {
        var sitemapLinksCount = links.Count(l =>
            l.CrawlingLinkSource == CrawlingLinkSource.Sitemap ||
            l.CrawlingLinkSource == CrawlingLinkSource.WebsiteAndSitemap);

        var crawlingLinksCount =
            links.Count(l =>
                l.CrawlingLinkSource == CrawlingLinkSource.Website ||
                l.CrawlingLinkSource == CrawlingLinkSource.WebsiteAndSitemap);

        _consoleWrapper.WriteLine($"Links in sitemap: {sitemapLinksCount}");
        _consoleWrapper.WriteLine($"Links after crawling: {crawlingLinksCount}");

        _consoleWrapper.WriteLine();
    }
}