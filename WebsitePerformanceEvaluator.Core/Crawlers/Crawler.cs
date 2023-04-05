using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Data.Enums;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private WebsiteCrawler WebsiteCrawler { get; }
    private SitemapCrawler SitemapCrawler { get; }
    private IMemoryCache MemoryCache { get; }
    private HttpClientService HttpClientService { get; }
    private LinkFilter LinkFilter { get; }


    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler, IMemoryCache
        memoryCache, HttpClientService httpClientService, LinkFilter linkFilter)
    {
        WebsiteCrawler = websiteCrawler;
        SitemapCrawler = sitemapCrawler;
        MemoryCache = memoryCache;
        HttpClientService = httpClientService;
        LinkFilter = linkFilter;
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

        var crawlingResult = (await crawlingTask)
            .Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Website));
        var sitemapResult = (await sitemapTask)
            .Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Sitemap));
        
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
            .Select(link => new Tuple<string, int>(link, HttpClientService.GetTimeResponse(link)))
            .ForAll(result.Add);


        return result;
    }


    private async Task<IEnumerable<string>> GetLinksByCrawling(string url)
    {
        var casheKey = url + "crawling";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result!;
        }

        result = await WebsiteCrawler.CrawlWebsiteToFindLinks(url);
        result = LinkFilter.FilterLinks(result, url);
        
        var linksByCrawling = result.ToList();

        MemoryCache.Set(casheKey, linksByCrawling);

        return linksByCrawling;
    }

    private async Task<IEnumerable<string>> GetSitemapLinks(string url)
    {
        var casheKey = url + "sitemap";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result!;
        }

        result = await SitemapCrawler.GetAllUrlsFromSitemap(url);
        result = LinkFilter.FilterLinks(result, url);

        var sitemapLinks = result.ToList();
        MemoryCache.Set(casheKey, sitemapLinks);

        return sitemapLinks;
    }
}