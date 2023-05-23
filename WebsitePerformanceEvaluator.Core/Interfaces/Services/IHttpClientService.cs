using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface IHttpClientService
{
    public Task<HtmlDocument> GetDocumentAsync(LinkPerformance link);
    
    public Task<long> GetTimeResponseAsync(string url);
    
    public Task<string> DownloadFileAsync(string fileUrl);
}