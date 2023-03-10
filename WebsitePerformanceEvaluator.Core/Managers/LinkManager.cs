using Microsoft.Extensions.Caching.Memory;
using WebsitePerformanceEvaluator.Core.Extensions;
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
    public IEnumerable<Tuple<string, int>> GetLinksWithTimeResponse(string url)
    {
        var crawlingResult = GetLinksByCrawling(url);
        var sitemapResult = GetSitemapLinks(url);
        
        var union = crawlingResult.Union(sitemapResult).Distinct();
        
        var result = union.AsParallel().Select(link => new Tuple<string, int>(link, ClientService.GetTimeResponse(link))).ToList();
        
        return result;
    }
    public IEnumerable<string> GetLinksByCrawling(string url)
    {
        var casheKey = url + "crawling";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));
        
        result = ClientService.CrawlWebsiteToFindLinks(url).ApplyFilters(url);
            
        MemoryCache.Set(casheKey, result, cacheEntryOptions);

        return result;
    }
    public IEnumerable<string> GetSitemapLinks(string url)
    {
        var casheKey = url + "sitemap";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(3));
        
        result = SitemapService.GetAllUrlsFromSitemap(url).ApplyFilters(url);
            
        MemoryCache.Set(casheKey, result, cacheEntryOptions);

        return result;
    }
}