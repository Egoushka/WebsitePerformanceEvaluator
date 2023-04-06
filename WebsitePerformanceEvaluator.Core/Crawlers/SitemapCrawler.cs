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
    private readonly ILogger _logger;
    private readonly HttpClientService _clientService;
    private readonly XmlParser _parser;
    private readonly LinkFilter _filter;
    public SitemapCrawler(ILogger logger, HttpClientService httpClientService, XmlParser xmlParser,
        LinkFilter linkFilter)
    {
        _logger = logger;
        _clientService = httpClientService;
        _parser = xmlParser;
        _filter = linkFilter;
    }

    public async Task<IEnumerable<LinkPerformanceResult>> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = await GetSitemap(baseUrl);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = _parser.GetLinks(xmlSitemapList);
        
        var filteredLinks = _filter.FilterLinks(urls, baseUrl)
            .RemoveLastSlashFromLinks()
            .AddBaseUrl(baseUrl);

        return filteredLinks;
    }

    private async Task<XmlDocument> GetSitemap(string baseUrl)
    {
        var uri = new Uri(baseUrl);
        baseUrl = uri.Scheme + "://" + uri.Host;

        var sitemapUrl = $"{baseUrl}/sitemap.xml";

        var sitemapXmlDocument = await GetSitemapXmlDocument(sitemapUrl);

        return sitemapXmlDocument.DocumentElement != null ? sitemapXmlDocument : new XmlDocument();
    }

    private async Task<XmlDocument> GetSitemapXmlDocument(string sitemapUrl)
    {
        var sitemapString = await _clientService.DownloadFile(sitemapUrl);
        var sitemapXmlDocument = new XmlDocument();
        try
        {
            sitemapXmlDocument.LoadXml(sitemapString);
        }
        catch (Exception)
        {
            _logger.Error("Error while parsing sitemap, sitemap will be ignored");
            return new XmlDocument();
        }

        return sitemapXmlDocument;
    }
}