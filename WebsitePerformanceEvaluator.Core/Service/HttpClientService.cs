using System.Diagnostics;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Interfaces;

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

    public async Task<HtmlDocument> GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = await _client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();

        doc.LoadHtml(html);

        return doc;
    }

    public async Task<int> GetTimeResponse(string url)
    {
        var time = 0;
        try
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            await _client.GetAsync(url);
            stopWatch.Stop();

            time = (int)stopWatch.ElapsedMilliseconds;
        }
        catch (Exception)
        {
            _logger.Error("Error while getting response time");
        }

        return time;
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