using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Models.Enums;
using WebsitePerformanceEvaluator.Data.Repository;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private readonly SitemapCrawler _sitemapCrawler;
    private readonly WebsiteCrawler _websiteCrawler;
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;

    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler, ILinkPerformanceRepository linkPerformanceRepository)
    {
        _websiteCrawler = websiteCrawler;
        _sitemapCrawler = sitemapCrawler;
        _linkPerformanceRepository = linkPerformanceRepository;
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
                Link = item.Link,
                CrawlingLinkSource = CrawlingLinkSource.WebsiteAndSitemap,
                TimeResponseMs = item.TimeResponseMs
            });

        var result = matches
            .Concat(crawlingResult.Except(sitemapResult))
            .Concat(sitemapResult.Except(crawlingResult));
        
        return await _linkPerformanceRepository.AddRangeAsync(result);
    }
}