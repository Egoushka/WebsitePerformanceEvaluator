using System.Xml;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class SitemapCrawler
{
    private readonly HttpClientService _clientService;
    private readonly LinkFilter _filter;
    private readonly LinkHelper _linkHelper;
    private readonly ILogger _logger;
    private readonly XmlParser _parser;

    public SitemapCrawler(ILogger logger, HttpClientService httpClientService, XmlParser xmlParser,
        LinkFilter linkFilter, LinkHelper linkHelper)
    {
        _logger = logger;
        _clientService = httpClientService;
        _parser = xmlParser;
        _filter = linkFilter;
        _linkHelper = linkHelper;
    }

    public async Task<IEnumerable<LinkPerformance>> FindLinksAsync(string url)
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
            _logger.Error("Error while parsing sitemap, sitemap will be ignored");
            return new XmlDocument();
        }

        return result;
    }
}