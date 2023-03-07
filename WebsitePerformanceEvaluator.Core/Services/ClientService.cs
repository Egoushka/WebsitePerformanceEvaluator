using System.Net;
using System.Text;
using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService : IClientService
{

    public ClientService()
    {
    }
    public async Task<XmlDocument> GetSitemapAsync(string baseUrl)
    {
        var sitemapURL = baseUrl + "/sitemap.xml";
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
}