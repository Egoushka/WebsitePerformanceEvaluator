using System.Xml;
using Serilog;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Crawlers;

public class SitemapCrawler
{
    private ILogger Logger { get; }
    private HttpClientService ClientService { get; }
    private XmlParser Parser { get; }
    private LinkFilter Filter { get; }

    public SitemapCrawler(ILogger logger, HttpClientService httpClientService, XmlParser xmlParser,
        LinkFilter linkFilter)
    {
        Logger = logger;
        ClientService = httpClientService;
        Parser = xmlParser;
        Filter = linkFilter;
    }

    public async Task<IEnumerable<string>> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = await GetSitemap(baseUrl);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = Parser.GetLinks(xmlSitemapList);
        var filteredLinks = Filter.FilterLinks(urls, baseUrl);

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
        var sitemapString = await ClientService.DownloadFile(sitemapUrl);
        var sitemapXmlDocument = new XmlDocument();
        try
        {
            sitemapXmlDocument.LoadXml(sitemapString);
        }
        catch (Exception)
        {
            Logger.Error("Error while parsing sitemap, sitemap will be ignored");
            return new XmlDocument();
        }

        return sitemapXmlDocument;
    }
}