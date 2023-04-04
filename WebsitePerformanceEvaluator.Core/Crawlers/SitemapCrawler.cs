using System.Xml;
using Serilog;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class SitemapCrawler
{

    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    
    public SitemapCrawler(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetAllUrlsFromSitemap(string baseUrl)
    {
        _logger.Information("Start getting sitemap links");
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
        var sitemapString = await DownloadSitemap(sitemapUrl);
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

        _logger.Information("Sitemap downloaded successfully");
        return sitemapXmlDocument;
    }

    private async Task<string> DownloadSitemap(string sitemapUrl)
    {
        string sitemapString;
        try
        {
            sitemapString = await _httpClient.GetStringAsync(sitemapUrl);
        }
        catch (Exception e)
        {
            _logger.Error("Error while downloading sitemap, sitemap will be ignored");
            return "";
        }

        return sitemapString;
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