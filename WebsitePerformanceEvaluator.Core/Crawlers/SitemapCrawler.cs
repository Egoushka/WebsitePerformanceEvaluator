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
    private readonly LinkHelper _linkHelper;
    public SitemapCrawler(ILogger logger, HttpClientService httpClientService, XmlParser xmlParser,
        LinkFilter linkFilter, LinkHelper linkHelper)
    {
        _logger = logger;
        _clientService = httpClientService;
        _parser = xmlParser;
        _filter = linkFilter;
        _linkHelper = linkHelper;
    }

    public async Task<IEnumerable<LinkPerformance>> FindLinks(string url)
    {
        var sitemapXml = await GetSitemap(url);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = _parser.GetLinks(xmlSitemapList);
        
        
        var filteredUrls = _filter.FilterLinks(urls, url);
        
        filteredUrls = _linkHelper.RemoveLastSlashFromLinks(filteredUrls);

        var result = await AddTimeToLinks(filteredUrls);

        return result;
    }

    private async Task<XmlDocument> GetSitemap(string url)
    {
        var uri = new Uri(url);
        url = uri.Scheme + "://" + uri.Host;

        var sitemapUrl = $"{url}/sitemap.xml";

        var sitemapXmlDocument = await GetSitemapXml(sitemapUrl);

        if (sitemapXmlDocument.DocumentElement == null)
        {
            sitemapXmlDocument = new XmlDocument();
        }
        
        return sitemapXmlDocument;
    }

    private async Task<XmlDocument> GetSitemapXml(string sitemapUrl)
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
    
    private async Task<IEnumerable<LinkPerformance>> AddTimeToLinks(IEnumerable<LinkPerformance> links)
    {
        foreach (var link in links)
        {
            var time = await _clientService.GetTimeResponse(link.Link);
            
            link.TimeResponse = time;
        }
        
        return links;
    }
}