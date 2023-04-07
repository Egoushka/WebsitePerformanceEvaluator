using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

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
    public async Task<IEnumerable<LinkPerformance>> CrawlWebsiteAndSitemapAsync(string url)
    {
        var crawlingTask = Task.Run(() => _websiteCrawler.FindLinksAsync(url));
        var sitemapTask = Task.Run(() => _sitemapCrawler.FindLinksAsync(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;
        
        var matches = crawlingResult.Intersect(sitemapResult)
            .Select(item => new LinkPerformance
        {
            Link = item.Link,
            CrawlingLinkType = CrawlingLinkType.Website | CrawlingLinkType.Sitemap,
            TimeResponse = item.TimeResponse
        });

        var result = matches
            .Concat(crawlingResult.Except(sitemapResult))
            .Concat(sitemapResult.Except(crawlingResult));

        return result;
    }
}