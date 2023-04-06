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
    public async Task<IEnumerable<LinkPerformanceResult>> GetLinksByCrawlingAndSitemap(string url)
    {
        var crawlingTask = Task.Run(() => GetLinksByCrawling(url));
        var sitemapTask = Task.Run(() => GetSitemapLinks(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;

        var result = new List<LinkPerformanceResult>();
        result.AddRange(sitemapResult);
        result.AddRange(crawlingResult);

        return result;
    }
    private async Task<IEnumerable<LinkPerformanceResult>> GetLinksByCrawling(string url)
    {
        var result = await _websiteCrawler.FindLinks(url);

        return result;
    }

    private async Task<IEnumerable<LinkPerformanceResult>> GetSitemapLinks(string url)
    {
        var result = await _sitemapCrawler.GetAllUrlsFromSitemap(url);

        return result;
    }
}