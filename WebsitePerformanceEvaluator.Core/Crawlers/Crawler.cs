using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Extensions;
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


    public async Task<IEnumerable<string>> GetLinksByCrawling(string url)
    {
        var casheKey = url + "crawling";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result!;
        }

        result = (await WebsiteCrawler.CrawlWebsiteToFindLinks(url)).ApplyFilters(url);
        var linksByCrawling = result.ToList();

        MemoryCache.Set(casheKey, linksByCrawling);

        return linksByCrawling;
    }

    public async Task<IEnumerable<string>> GetSitemapLinks(string url)
    {
        var casheKey = url + "sitemap";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result!;
        }

        result = (await SitemapCrawler.GetAllUrlsFromSitemap(url)).ApplyFilters(url);

        var sitemapLinks = result.ToList();
        MemoryCache.Set(casheKey, sitemapLinks);

        return sitemapLinks;
    }
}