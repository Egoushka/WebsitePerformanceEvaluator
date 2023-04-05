using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Data.Enums;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private WebsiteCrawler WebsiteCrawler { get; }
    private SitemapCrawler SitemapCrawler { get; }
    private IMemoryCache MemoryCache { get; }
    private HttpClientService HttpClientService { get; }

    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler, IMemoryCache
        memoryCache, HttpClientService httpClientService)
    {
        WebsiteCrawler = websiteCrawler;
        SitemapCrawler = sitemapCrawler;
        MemoryCache = memoryCache;
        HttpClientService = httpClientService;
    }

    public async Task<IEnumerable<Tuple<string, CrawlingLinkType>>> GetLinksByCrawlingAndSitemap(string url)
    {
        var casheKey = url + "crawlingAndSitemap";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<Tuple<string, CrawlingLinkType>>? result))
        {
            return result!;
        }

        var crawlingTask = Task.Run(() => GetLinksByCrawling(url));
        var sitemapTask = Task.Run(() => GetSitemapLinks(url));

        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;
        
        var union = crawlingResult.Union(sitemapResult).Distinct();
        
        return union;
    }

    public async Task<IEnumerable<Tuple<string, int>>> GetLinksWithTimeResponse(string url)
    {
        var crawlingTask = Task.Run(() => GetLinksByCrawling(url));
        var sitemapTask = Task.Run(() => GetSitemapLinks(url));


        var crawlingResult = await crawlingTask;
        var sitemapResult = await sitemapTask;

        var union = crawlingResult.Union(sitemapResult).Distinct();

        var result = new List<Tuple<string, int>>();

        union
            .AsParallel()
            .Select(link => new Tuple<string, int>(link.Item1, HttpClientService.GetTimeResponse(link.Item1)))
            .ForAll(result.Add);

        return result;
    }


    private async Task<List<Tuple<string, CrawlingLinkType>>> GetLinksByCrawling(string url)
    {
        var casheKey = url + "crawling";
        if (MemoryCache.TryGetValue(casheKey, out List<Tuple<string, CrawlingLinkType>>? result))
        {
            return result!;
        }

        var rawLinks = await WebsiteCrawler.CrawlWebsiteToFindLinks(url);
        
        result = rawLinks.Select(link=> new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Website)).ToList();

        MemoryCache.Set(casheKey, result);

        return result;
    }

    private async Task<List<Tuple<string, CrawlingLinkType>>> GetSitemapLinks(string url)
    {
        var casheKey = url + "sitemap";

        if (MemoryCache.TryGetValue(casheKey, out List<Tuple<string, CrawlingLinkType>>? result))
        {
            return result!;
        }

        var rawLinks = await SitemapCrawler.GetAllUrlsFromSitemap(url);

        result = rawLinks.Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Sitemap)).ToList();
        MemoryCache.Set(casheKey, result);

        return result;
    }
}