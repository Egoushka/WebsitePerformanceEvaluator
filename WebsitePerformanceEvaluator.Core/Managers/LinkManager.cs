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

    public async Task<IEnumerable<Tuple<string, int>>> GetLinksWithTimeResponse(string url)
    {
        var crawlingResult = await GetLinksByCrawling(url);
        var sitemapResult = GetSitemapLinks(url);
        var union = crawlingResult.Union(sitemapResult).Distinct();

        var result = new List<Tuple<string, int>>();
        var tasks = new List<Task>();

        foreach (var link in union)
        {
            tasks.Add(Task.Run(() =>
            {
                var time = ClientService.GetTimeResponse(link);
                result.Add(new Tuple<string, int>(link, time));
            }));
        }

        await Task.WhenAll(tasks);

        return result;
    }

    public async Task<IEnumerable<string>> GetLinksByCrawling(string url)
    {
        var casheKey = url + "crawling";
        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }

        result = (await ClientService.CrawlWebsiteToFindLinks(url)).ApplyFilters(url);
        MemoryCache.Set(casheKey, result);

        return result;
    }

    public IEnumerable<string> GetSitemapLinks(string url)
    {
        var casheKey = url + "sitemap";

        if (MemoryCache.TryGetValue(casheKey, out IEnumerable<string>? result))
        {
            return result;
        }

        result = SitemapService.GetAllUrlsFromSitemap(url).ApplyFilters(url);

        var sitemapLinks = result.ToList();
        MemoryCache.Set(casheKey, sitemapLinks);

        return sitemapLinks;
    }
}