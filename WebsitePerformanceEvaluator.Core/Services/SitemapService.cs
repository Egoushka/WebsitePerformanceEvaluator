using System.Net;
using System.Text;
using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class SitemapService : ISitemapService
{
    private IClientService ClientService { get; set; }

    public SitemapService(IClientService сlientService)
    {
        ClientService = сlientService;
    }

    public IEnumerable<string> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = GetSitemap(baseUrl);
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");

        var urls = GetRawUrlsFromSitemap(xmlSitemapList);

        return urls;
    }

    private XmlDocument GetSitemap(string baseUrl)
    {
        var uri = new Uri(baseUrl);
        baseUrl = uri.Scheme + "://" + uri.Host;

        var sitemapUrl = $"{baseUrl}/sitemap.xml";

        var sitemapXmlDocument = GetSitemapXmlDocument(sitemapUrl);
        
        return sitemapXmlDocument.DocumentElement == null ? new XmlDocument() : sitemapXmlDocument;
    }
    private XmlDocument GetSitemapXmlDocument(string sitemapUrl)
    {
        var sitemapString = DownloadSitemap(sitemapUrl);
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
    private string DownloadSitemap(string sitemapUrl)
    {
        var wc = new WebClient
        {
            Encoding = Encoding.UTF8
        };
        string sitemapString;
        try
        {
            sitemapString = wc.DownloadString(sitemapUrl);
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