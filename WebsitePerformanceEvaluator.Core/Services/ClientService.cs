using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService : IClientService
{
    public XmlDocument GetSitemap(string baseUrl)
    {
        var sitemapUrl = $"{baseUrl}sitemap.xml";
        
        //I had troubles with HttpClient, so I had to use WebClient
        var wc = new WebClient
        {
            Encoding = Encoding.UTF8
        };
        var sitemapString = "";
        try
        {
            sitemapString = wc.DownloadString(sitemapUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while getting sitemap, sitemap will be ignored: " + e.Message);
            return new XmlDocument();
        }
        var sitemapXmlDocument = new XmlDocument();
        sitemapXmlDocument.LoadXml(sitemapString);

        return sitemapXmlDocument;
    }
    public IEnumerable<string> CrawlToFindLinks(string url)
    {
        var doc = GetDocument(url);
        
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link =>
            link.Attributes["href"].Value);
    }
    private HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
    public int GetTimeResponse(string url)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        var timer = new Stopwatch();

        timer.Start();
        try
        {
            var response = (HttpWebResponse)request.GetResponse();
            response.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while getting response time of {url}, response time will be ignored. Error: {e.Message}");
        }

        timer.Stop();

        var timeTaken = timer.Elapsed;
        
        return timeTaken.Milliseconds;
    }
}