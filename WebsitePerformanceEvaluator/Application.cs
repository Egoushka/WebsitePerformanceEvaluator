using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator;

public class Application
{
    public ISitemapService SitemapService { get; set; }
    public Application(ISitemapService sitemapService)
    {
        SitemapService = sitemapService;
    }

    public async Task Run()
    {
        var result = await SitemapService.GetAllUrlsFromSitemapAsync("https://seoagilitytools.com");
        
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }
}