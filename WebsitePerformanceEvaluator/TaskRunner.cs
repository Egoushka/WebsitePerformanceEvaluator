using WebsitePerformanceEvaluator.Core.Interfaces.Managers;

namespace WebsitePerformanceEvaluator;

public class TaskRunner
{
    private ILinkManager LinkManager { get; set; }
    private int LinksCountInSitemap { get; set; }
    private int LinksCountAfterCrawling { get; set; }

    public TaskRunner(ILinkManager linkManager)
    {
        LinkManager = linkManager;
    }
    public void Start()
    {   
        var url = ConsoleHelper.GetInput("Enter url:");
        var url1 = "https://www.yourdigitalresource.com/";
        var linksByCrawling = LinkManager.GetLinksByCrawling(url).ToList();
        var sitemapLinks = LinkManager.GetSitemapLinks(url).ToList();
        
        LinksCountInSitemap = sitemapLinks.Count;
        LinksCountAfterCrawling = linksByCrawling.Count;
        
        PrintLinksInCrawlingNotInSitemap(linksByCrawling, sitemapLinks);
        PrintLinksInSitemapNotInCrawling(linksByCrawling, sitemapLinks);
        PrintLinksWithTimeResponse(url);
        
        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");
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
            ConsoleHelper.PrintTable(new List<string> {"Link"}, linksInCrawlingNotInSitemap);
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
            ConsoleHelper.PrintTable(new List<string> {"Link"}, linksInSitemapNotInCrawling);
        }

        Console.WriteLine();
    }

    private void PrintLinksWithTimeResponse(string url)
    {
        var linksWithTimeResponse = LinkManager.GetLinksWithTimeResponse(url)
            .OrderBy(item => item.Item2)
            .ToList();
        Console.WriteLine("Links with time response:");
        
        ConsoleHelper.PrintTable(new List<string> {"Link", "Time(ms)"}, linksWithTimeResponse);
        Console.WriteLine();
    }
}