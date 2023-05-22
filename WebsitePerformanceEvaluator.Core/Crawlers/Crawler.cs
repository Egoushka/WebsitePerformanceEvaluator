using WebsitePerformanceEvaluator.Domain.Enums;
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private readonly SitemapCrawler _sitemapCrawler;
    private readonly WebsiteCrawler _websiteCrawler;

    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler)
    {
        _websiteCrawler = websiteCrawler;
        _sitemapCrawler = sitemapCrawler;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> CrawlWebsiteAndSitemapAsync(string url)
    {
        var crawlingTask = Task.Run(() => _websiteCrawler.FindLinksAsync(url));
        var sitemapTask = Task.Run(() => _sitemapCrawler.FindLinksAsync(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;

        var matches = crawlingResult.Intersect(sitemapResult)
            .Select(item => new LinkPerformance
            {
                Url = item.Url,
                CrawlingLinkSource = CrawlingLinkSource.WebsiteAndSitemap,
                TimeResponseMs = item.TimeResponseMs
            });

        var result = matches
            .Concat(crawlingResult.Except(sitemapResult))
            .Concat(sitemapResult.Except(crawlingResult));

        return result;
    }
}