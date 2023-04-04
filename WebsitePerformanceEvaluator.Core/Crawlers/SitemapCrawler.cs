using System.Xml;
using Serilog;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class SitemapCrawler
{
    private readonly ILogger _logger;
    private readonly HttpClientService _httpClientService;
    
    public SitemapCrawler(ILogger logger, HttpClientService httpClientService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
    }

    public async Task<IEnumerable<string>> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = await GetSitemap(baseUrl);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = GetRawUrlsFromSitemap(xmlSitemapList);

        return urls;
    }

    private async Task<XmlDocument> GetSitemap(string baseUrl)
    {
        var uri = new Uri(baseUrl);
        baseUrl = uri.Scheme + "://" + uri.Host;

        var sitemapUrl = $"{baseUrl}/sitemap.xml";

        var sitemapXmlDocument = await GetSitemapXmlDocument(sitemapUrl);

        return sitemapXmlDocument.DocumentElement == null ? new XmlDocument() : sitemapXmlDocument;
    }

    private async Task<XmlDocument> GetSitemapXmlDocument(string sitemapUrl)
    {
        var sitemapString = await _httpClientService.DownloadSitemap(sitemapUrl);
        var sitemapXmlDocument = new XmlDocument();
        try
        {
            sitemapXmlDocument.LoadXml(sitemapString);
        }
        catch (Exception e)
        {
            _logger.Error("Error while parsing sitemap, sitemap will be ignored");
            return new XmlDocument();
        }

        return sitemapXmlDocument;
    }

   

    private IEnumerable<string> GetRawUrlsFromSitemap(XmlNodeList xmlSitemapList)
    {
        var result = new List<string>();

        foreach (XmlNode node in xmlSitemapList)
        {
            if (node["loc"] != null)
            {
                result.Add(node["loc"]!.InnerText);
            }
        }

        return result;
    }
}