using System.Xml;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class SitemapService : ISitemapService
{
    public IClientService ClientService { get; set; }

    public SitemapService(IClientService сlientService)
    {
        ClientService = сlientService;
    }
    
    public async Task<List<string>> GetAllUrlsFromSitemapAsync(string baseUrl)
    {
        var sitemapXml = await ClientService.GetSitemapAsync(baseUrl);
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