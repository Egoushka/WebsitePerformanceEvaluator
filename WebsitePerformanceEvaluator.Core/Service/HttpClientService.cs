using System.Diagnostics;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;

namespace WebsitePerformanceEvaluator.Core.Service;

public class HttpClientService
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public HttpClientService(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _client = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<HtmlDocument> GetDocument(LinkPerformance link)
    {
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();
        var doc = await GetDocument(link.Link);
        stopWatch.Stop();
        
        var time = stopWatch.ElapsedMilliseconds;
        link.TimeResponse = time;
        
        return doc;
    }
    public async Task<HtmlDocument> GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = await _client.GetAsync(url);
        
        var html = await response.Content.ReadAsStringAsync();

        doc.LoadHtml(html);

        return doc;
    }

    public async Task<long> GetTimeResponse(string url)
    {
        var stopWatch = new Stopwatch();

        stopWatch.Start();
        await _client.GetAsync(url);
        stopWatch.Stop();

        return stopWatch.ElapsedMilliseconds;
    }

    public async Task<string> DownloadFile(string fileUrl)
    {
        try
        {
            return await _client.GetStringAsync(fileUrl);
        }
        catch (Exception)
        {
            _logger.Error("Error while downloading file");
            return string.Empty;
        }
    }
}