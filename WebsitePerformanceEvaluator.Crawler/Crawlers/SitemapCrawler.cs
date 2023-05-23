using System.Xml;
using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Core.Interfaces.Crawlers;
using WebsitePerformanceEvaluator.Core.Interfaces.FIlters;
using WebsitePerformanceEvaluator.Core.Interfaces.Helpers;
using WebsitePerformanceEvaluator.Core.Interfaces.Parsers;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Crawler.Filters;
using WebsitePerformanceEvaluator.Crawler.Helpers;
using WebsitePerformanceEvaluator.Crawler.Parsers;
using WebsitePerformanceEvaluator.Crawler.Services;

namespace WebsitePerformanceEvaluator.Crawler.Crawlers;

public class SitemapCrawler : ICrawler, ISitemapCrawler
{
    private readonly IHttpClientService _clientService;
    private readonly ILinkFilter _filter;
    private readonly ILinkHelper _linkHelper;
    private readonly ILogger _logger;
    private readonly IXmlParser _parser;

    public SitemapCrawler()
    {
    }

    public SitemapCrawler(ILogger logger, IHttpClientService httpClientService, IXmlParser xmlParser,
        ILinkFilter linkFilter, ILinkHelper linkHelper)
    {
        _logger = logger;
        _clientService = httpClientService;
        _parser = xmlParser;
        _filter = linkFilter;
        _linkHelper = linkHelper;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url)
    {
        var sitemapXml = await GetSitemapAsync(url);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = _parser.GetLinks(xmlSitemapList);
        var filteredUrls = _filter.FilterLinks(urls, url);

        filteredUrls = _linkHelper.RemoveLastSlashFromLinks(filteredUrls);

        return await _linkHelper.AddResponseTimeAsync(filteredUrls);
    }

    private async Task<XmlDocument> GetSitemapAsync(string url)
    {
        var uri = new Uri(url);
        url = uri.Scheme + "://" + uri.Host;

        var sitemapUrl = $"{url}/sitemap.xml";

        var result = await GetSitemapXmlAsync(sitemapUrl);

        if (result.DocumentElement == null)
        {
            result = new XmlDocument();
        }

        return result;
    }

    private async Task<XmlDocument> GetSitemapXmlAsync(string sitemapUrl)
    {
        var sitemapString = await _clientService.DownloadFileAsync(sitemapUrl);

        var result = new XmlDocument();

        try
        {
            result.LoadXml(sitemapString);
        }
        catch (Exception)
        {
            _logger.LogError("Error while parsing sitemap, sitemap will be ignored");
            return new XmlDocument();
        }

        return result;
    }
}