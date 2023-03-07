using WebsitePerformanceEvaluator.Core.Interfaces.Managers;

namespace WebsitePerformanceEvaluator;

public class Application
{
    private ILinkManager LinkManager { get; set; }
    private int LinksCountInSitemap { get; set; }
    private int LinksCountAfterCrawling { get; set; }
    public Application(ILinkManager linkManager)
    {
        LinkManager = linkManager;
    }

    public void Run()
    {
        const string url = "https://www.w3schools.com/";
        PrintLinksInCrawlingNotInSitemap(url);
        PrintLinksInSitemapNotInCrawling(url);
        PrintLinksWithTimeResponse(url);
        
        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");
    }

    private void PrintLinksInCrawlingNotInSitemap(string url)
    {
        var linksInCrawlingNotInSitemap = LinkManager.GetLinksThatExistInCrawlingButNotInSitemap(url).ToList();
        Console.WriteLine("Links in crawling not in sitemap:");
        if (!linksInCrawlingNotInSitemap.Any())
        {
            Console.WriteLine("No links found");
        }
        else
        {
            LinksCountInSitemap = linksInCrawlingNotInSitemap.Count();
            ConsoleHelper.PrintTable(new List<string> {"Link"}, linksInCrawlingNotInSitemap);
        }
    }

    private void PrintLinksInSitemapNotInCrawling(string url)
    {
        var linksInSitemapNotInCrawling = LinkManager.GetLinksThatExistInSitemapButNotInCrawling(url).ToList();
        Console.WriteLine("Links in sitemap not in crawling:");
        if (!linksInSitemapNotInCrawling.Any())
        {
            Console.WriteLine("No links found");
        }
        else
        {
            LinksCountAfterCrawling = linksInSitemapNotInCrawling.Count();
            ConsoleHelper.PrintTable(new List<string> {"Links"}, linksInSitemapNotInCrawling);
        }
    }

    private void PrintLinksWithTimeResponse(string url)
    {
        var linksWithTimeResponse = LinkManager.GetLinksWithTimeResponse(url).ToList();
        Console.WriteLine("Links with time response:");
        if (!linksWithTimeResponse.Any())
        {
            Console.WriteLine("No links found");
        }
        else
        {
            ConsoleHelper.PrintTable(new List<string> {"Link", "Time"}, linksWithTimeResponse);
        }
    }
    
}