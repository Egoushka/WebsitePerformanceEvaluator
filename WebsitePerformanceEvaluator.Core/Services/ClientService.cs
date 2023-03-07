using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService : IClientService
{

    public ClientService()
    {
    }
    public async Task<XmlDocument> GetSitemapAsync(string baseUrl)
    {
        var sitemapURL = $"{baseUrl}/sitemap.xml";
        
        //I had troubles with HttpClient, so I had to use WebClient
        var wc = new WebClient
        {
            Encoding = Encoding.UTF8
        };
        var sitemapString = wc.DownloadString(sitemapURL);

        var sitemapXmlDocument = new XmlDocument();
        sitemapXmlDocument.LoadXml(sitemapString);

        return sitemapXmlDocument;
    }
    public List<string> CrawlToFindLinks(string url)
    {
        var doc = GetDocument(url);
        
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }
        var baseUri = new Uri(url);
        
        return linkNodes.Select(link => 
                link.Attributes["href"].Value)
            .Where(href => href.StartsWith('/'))
            .Distinct()
            .Select(href => new Uri(baseUri, href).AbsoluteUri)
            .ToList();
    }
    private HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
}