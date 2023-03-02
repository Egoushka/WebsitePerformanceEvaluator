using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Querier.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class SitemapService : ISItemapService
{
    public ClientService ClientService { get; set; }

    public SitemapService()
    {
        ClientService = new ClientService();
    }
    
    public async Task<List<string>> GetAllUrlsFromSitemap(string baseUrl)
    {
        var sitemapXml = await ClientService.GetSitemap(baseUrl);
        
        var xmlSitemapList = sitemapXml.GetElementsByTagName("url");
        var urls= new List<string>();
        foreach (XmlNode node in xmlSitemapList)
        {
            if (node["loc"] != null)
            {
                urls.Add(node["loc"].InnerText);
            }
        }

        return urls;
    }
}