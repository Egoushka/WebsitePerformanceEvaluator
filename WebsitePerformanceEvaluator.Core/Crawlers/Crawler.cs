using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Data.Enums;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private WebsiteCrawler WebsiteCrawler { get; }
    private SitemapCrawler SitemapCrawler { get; }
    private HttpClientService HttpClientService { get; }

    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler,  HttpClientService httpClientService)
    {
        WebsiteCrawler = websiteCrawler;
        SitemapCrawler = sitemapCrawler;
        HttpClientService = httpClientService;
    }
    public async Task<IEnumerable<Tuple<string, int>>> GetLinksWithTimeResponse(string url)
    {
        var links = await GetLinksByCrawlingAndSitemap(url);

        var result = links
            .AsParallel()
            .Select(async link =>
                new Tuple<string, int>(link.Item1, await HttpClientService.GetTimeResponse(link.Item1)));
        
        return await Task.WhenAll(result);
    }
    public async Task<IEnumerable<Tuple<string, CrawlingLinkType>>> GetLinksByCrawlingAndSitemap(string url)
    {
        var crawlingTask = Task.Run(() => GetLinksByCrawling(url));
        var sitemapTask = Task.Run(() => GetSitemapLinks(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;

        var union = crawlingResult.Union(sitemapResult).Distinct();

        return union;
    }
    private async Task<List<Tuple<string, CrawlingLinkType>>> GetLinksByCrawling(string url)
    {
        var rawLinks = await WebsiteCrawler.FindLinks(url);

        var result = rawLinks.Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Website))
            .ToList();

        return result;
    }

    private async Task<List<Tuple<string, CrawlingLinkType>>> GetSitemapLinks(string url)
    {
        var rawLinks = await SitemapCrawler.GetAllUrlsFromSitemap(url);

        var result = rawLinks.Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Sitemap))
            .ToList();

        return result;
    }
}