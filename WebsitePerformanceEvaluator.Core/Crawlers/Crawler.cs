using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private readonly WebsiteCrawler _websiteCrawler;
    private readonly SitemapCrawler _sitemapCrawler;
    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler)
    {
        _websiteCrawler = websiteCrawler;
        _sitemapCrawler = sitemapCrawler;
    }
    public async Task<IEnumerable<LinkPerformance>> CrawlWebsiteAndSitemap(string url)
    {
        var crawlingTask = Task.Run(() => CrawlWebsite(url));
        var sitemapTask = Task.Run(() => CrawlSitemap(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;

        var result = new List<LinkPerformance>();
        
        result.AddRange(sitemapResult);
        result.AddRange(crawlingResult);

        return result;
    }
    private async Task<IEnumerable<LinkPerformance>> CrawlWebsite(string url)
    {
        var result = await _websiteCrawler.FindLinks(url);

        return result;
    }

    private async Task<IEnumerable<LinkPerformance>> CrawlSitemap(string url)
    {
        var result = await _sitemapCrawler.FindLinks(url);

        return result;
    }
}