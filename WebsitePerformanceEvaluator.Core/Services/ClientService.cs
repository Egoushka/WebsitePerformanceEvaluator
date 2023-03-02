using System.Net;
using System.Text;
using System.Xml;

namespace WebsitePerformanceEvaluator.Querier.Services;

public class ClientService
{
    public async Task<XmlDocument> GetSitemap(string baseUrl)
    {
        var sitemapURL = baseUrl + "/sitemap.xml";

        var wc = new HttpClient();
        

        
        var response = await wc.GetAsync(sitemapURL);
        var sitemapXmlDocument = new XmlDocument();

        sitemapXmlDocument.LoadXml(await response.Content.ReadAsStringAsync());

        return sitemapXmlDocument;
    }
}