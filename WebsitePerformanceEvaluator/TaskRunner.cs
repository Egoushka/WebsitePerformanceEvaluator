
using WebsitePerformanceEvaluator.Core.Managers;

namespace WebsitePerformanceEvaluator;

public class TaskRunner
{
    private LinkManager LinkManager { get; set; }
    private int LinksCountInSitemap { get; set; }
    private int LinksCountAfterCrawling { get; set; }

    public TaskRunner(LinkManager linkManager)
    {
        LinkManager = linkManager;
    }

    public async Task Start()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        //var url = ConsoleHelper.GetInput("Enter url:");
        var url = "https://ukad-group.com/";
        
        watch.Start();
        var linksByCrawlingTask = Task.Run(() => LinkManager.GetLinksByCrawling(url));
        var sitemapLinksTask = Task.Run(() => LinkManager.GetSitemapLinks(url));

        var linksByCrawling = (await linksByCrawlingTask).ToList();
        var sitemapLinks = (await sitemapLinksTask).ToList();

        LinksCountInSitemap = sitemapLinks.Count;
        LinksCountAfterCrawling = linksByCrawling.Count;

        PrintLinksInCrawlingNotInSitemap(linksByCrawling, sitemapLinks);
        PrintLinksInSitemapNotInCrawling(linksByCrawling, sitemapLinks);
        await PrintLinksWithTimeResponse(url);


        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");
        watch.Stop();

        Console.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }

    private void PrintLinksInCrawlingNotInSitemap(List<string> crawlingLinks, List<string> sitemapLinks)
    {
        var linksInCrawlingNotInSitemap = crawlingLinks.Except(sitemapLinks).ToList();
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

    private void PrintLinksInSitemapNotInCrawling(List<string> crawlingLinks, List<string> sitemapLinks)
    {
        var linksInSitemapNotInCrawling = sitemapLinks.Except(crawlingLinks).ToList();
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
        var linksWithTimeResponse = (await LinkManager.GetLinksWithTimeResponse(url))
            .OrderBy(item => item.Item2)
            .ToList();
        Console.WriteLine("Links with time response:");

        ConsoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, linksWithTimeResponse);
        Console.WriteLine();
    }
}