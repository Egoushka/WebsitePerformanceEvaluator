using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core.Helpers;

public class LinkHelper
{
    private readonly HttpClientService _clientService;
    
    public LinkHelper(HttpClientService httpClientService)
    {
        _clientService = httpClientService;
    }
    
    public IEnumerable<LinkPerformance> AddBaseUrl(IEnumerable<LinkPerformance> links, string baseUrl)
    {
        foreach (var link in links)
        {
            link.Link = AddBaseUrl(link.Link, baseUrl);
        }
        
        return links;
    }
    
    public IEnumerable<LinkPerformance> RemoveLastSlashFromLinks(IEnumerable<LinkPerformance> links)
    {
        foreach (var link in links)
        {
            link.Link = RemoveLastSlashFromLink(link.Link);
        }
        
        return links;
    }
    
    public string AddBaseUrl(string link, string baseUrl)
    {
        return link.StartsWith("/") ? baseUrl[..^1] + link : link;
    }
    
    public string RemoveLastSlashFromLink(string link)
    {
        return link.EndsWith("/") ? link.Remove(link.Length - 1) : link;
    }
    
    public async Task<IEnumerable<LinkPerformance>> AddTimeToLinksAsync(IEnumerable<LinkPerformance> links)
    {
        foreach (var link in links)
        {
            var time = await _clientService.GetTimeResponseAsync(link.Link);
            
            link.TimeResponse = time;
        }
        
        return links;
    }
}