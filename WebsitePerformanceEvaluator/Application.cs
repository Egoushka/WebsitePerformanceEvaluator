using ConsoleTableExt;
using WebsitePerformanceEvaluator.Core.Interfaces.Managers;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Managers;

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
        var url = "https://www.w3schools.com/";
        PrintLinksInCrawlingNotInSitemap(url);
        PrintLinksInSitemapNotInCrawling(url);
        PrintLinksWithTimeResponse(url);
        
        Console.WriteLine($"Links in sitemap: {LinksCountInSitemap}");
        Console.WriteLine($"Links after crawling: {LinksCountAfterCrawling}");
    }

    private void PrintLinksInCrawlingNotInSitemap(string url)
    {
        var linksInCrawlingNotInSitemap = LinkManager.GetLinksThatExistInCrawlingButNotInSitemap(url);
        Console.WriteLine("Links in crawling not in sitemap:");
        if (linksInCrawlingNotInSitemap == null)
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
        var linksInSitemapNotInCrawling = LinkManager.GetLinksThatExistInSitemapButNotInCrawling(url);
        Console.WriteLine("Links in sitemap not in crawling:");
        if (linksInSitemapNotInCrawling == null)
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
        var linksWithTimeResponse = LinkManager.GetLinksWithTimeResponse(url);
        Console.WriteLine("Links with time response:");
        if (linksWithTimeResponse == null)
        {
            Console.WriteLine("No links found");
        }
        else
        {
            ConsoleHelper.PrintTable(new List<string> {"Link", "Time"}, linksWithTimeResponse.ToList());
        }
    }
    
}