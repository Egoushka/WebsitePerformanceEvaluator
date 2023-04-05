using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class Crawler
{
    private readonly WebsiteCrawler _websiteCrawler;
    private readonly SitemapCrawler _sitemapCrawler;
    private readonly HttpClientService _httpClientService;
    public Crawler(WebsiteCrawler websiteCrawler, SitemapCrawler sitemapCrawler,  HttpClientService httpClientService)
    {
        _websiteCrawler = websiteCrawler;
        _sitemapCrawler = sitemapCrawler;
        _httpClientService = httpClientService;
    }
    public async Task<IEnumerable<Tuple<string, int>>> GetLinksWithTimeResponse(string url)
    {
        var links = await GetLinksByCrawlingAndSitemap(url);

        var result = links
            .AsParallel()
            .Select(async link =>
                new Tuple<string, int>(link.Item1, await _httpClientService.GetTimeResponse(link.Item1)));
        
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
        var rawLinks = await _websiteCrawler.FindLinks(url);

        var result = rawLinks.Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Website))
            .ToList();

        return result;
    }

    private async Task<List<Tuple<string, CrawlingLinkType>>> GetSitemapLinks(string url)
    {
        var rawLinks = await _sitemapCrawler.GetAllUrlsFromSitemap(url);

        var result = rawLinks.Select(link => new Tuple<string, CrawlingLinkType>(link, CrawlingLinkType.Sitemap))
            .ToList();

        return result;
    }
}