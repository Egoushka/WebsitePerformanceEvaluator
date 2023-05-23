using WebsitePerformanceEvaluator.Core.Interfaces.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;

namespace WebsitePerformanceEvaluator.Crawler.Crawlers;

public class CombinedCrawler : ICombinedCrawler
{
    private readonly ISitemapCrawler _sitemapCrawler;
    private readonly IWebsiteCrawler _websiteCrawler;

    public CombinedCrawler(IWebsiteCrawler websiteCrawler, ISitemapCrawler sitemapCrawler)
    {
        _websiteCrawler = websiteCrawler;
        _sitemapCrawler = sitemapCrawler;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url)
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