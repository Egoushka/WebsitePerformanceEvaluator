using WebsitePerformanceEvaluator.Core.Services;
using WebsitePerformanceEvaluator.Crawler;

var url = "https://seoagilitytools.com/";

var sitemapService = new SitemapService();

var urlsBySitemap = await sitemapService.GetAllUrlsFromSitemap(url);
var urlsByCrawling = PageCrawler.GetLinks(url);

var allUrls = urlsBySitemap.Union(urlsByCrawling).Distinct().ToList();

foreach (var item in allUrls)
{
    Console.WriteLine(item);
}