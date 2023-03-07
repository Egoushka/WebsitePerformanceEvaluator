using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Interfaces.Managers;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Managers;

public class LinkManager : ILinkManager
{
    private IClientService ClientService { get; set; }
    private ISitemapService SitemapService { get; set; }
    private IMemoryCache MemoryCache { get; set; }

    public LinkManager(IClientService clientService, ISitemapService sitemapService, IMemoryCache
        memoryCache)
    {
        ClientService = clientService;
        SitemapService = sitemapService;
        MemoryCache = memoryCache;
    }

    public IEnumerable<string> GetLinksThatExistInSitemapButNotInCrawling(string url)
    {
        var casheKey = url + "sitemap";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));
        
        var crawlingResult = ClientService.CrawlToFindLinks(url);
        var sitemapResult = SitemapService.GetAllUrlsFromSitemap(url);
            
        result = sitemapResult.Except(crawlingResult);

        MemoryCache.Set(casheKey, result, cacheEntryOptions);

        return result;
    }
    public IEnumerable<string> GetLinksThatExistInCrawlingButNotInSitemap(string url)
    {
        var casheKey = url + "crawling";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));
        
        var crawlingResult = ClientService.CrawlToFindLinks(url);
        var sitemapResult = SitemapService.GetAllUrlsFromSitemap(url);
            
        result = crawlingResult.Except(sitemapResult);

        MemoryCache.Set(casheKey, result, cacheEntryOptions);

        return result;
    }
    public IEnumerable<Tuple<string, int>> GetLinksWithTimeResponse(string url)
    {
        var casheKey = url + "time";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<Tuple<string, int>>? result))
        {
            return result;
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));
        
        var crawlingResult = ClientService.CrawlToFindLinks(url);
        var sitemapResult = SitemapService.GetAllUrlsFromSitemap(url);
        
        var union = crawlingResult.Union(sitemapResult).Distinct();
        
        result = union.Select(link => new Tuple<string, int>(link, ClientService.GetTimeResponse(link)));

        MemoryCache.Set(casheKey, result, cacheEntryOptions);

        return result;
    }
}