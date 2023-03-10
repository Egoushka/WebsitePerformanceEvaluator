using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class SitemapService : ISitemapService
{

    private readonly HttpClient _httpClient = new();

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
        var sitemapString = await DownloadSitemap(sitemapUrl);
        var sitemapXmlDocument = new XmlDocument();
        try
        {
            sitemapXmlDocument.LoadXml(sitemapString);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while parsing sitemap, sitemap will be ignored: " + e.Message);
            return new XmlDocument();
        }

        Console.WriteLine("Sitemap was successfully parsed");
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
            Console.WriteLine("Error while getting sitemap, sitemap will be ignored: " + e.Message);
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