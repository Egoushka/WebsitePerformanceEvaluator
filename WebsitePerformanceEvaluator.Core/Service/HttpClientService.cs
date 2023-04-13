using System.Diagnostics;
using System.Text;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Data.Models;
using WebsitePerformanceEvaluator.Core.Interfaces;

namespace WebsitePerformanceEvaluator.Core.Service;

public class HttpClientService
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public HttpClientService()
    {
    }

    public HttpClientService(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _client = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public virtual async Task<HtmlDocument> GetDocumentAsync(LinkPerformance link)
    {
        var stopWatch = new Stopwatch();

        stopWatch.Start();
        var doc = await GetDocumentAsync(link.Link);
        stopWatch.Stop();

        var time = stopWatch.ElapsedMilliseconds;
        link.TimeResponseMs = (int)time;

        return doc;
    }

    private async Task<HtmlDocument> GetDocumentAsync(string url)
    {
        var doc = new HtmlDocument();

        using var response = await _client.GetAsync(url);

        var contentType = response.Content.Headers.ContentType;

        if (contentType.MediaType != "text/html")
        {
            return new HtmlDocument();
        }

        var charset = contentType.CharSet ?? Encoding.UTF8.WebName;
        var html = await response.Content.ReadAsStringAsync();

        doc.LoadHtml(html);
        doc.OptionDefaultStreamEncoding = Encoding.GetEncoding(charset);

        return doc;
    }

    public virtual async Task<long> GetTimeResponseAsync(string url)
    {
        var stopWatch = new Stopwatch();

        stopWatch.Start();
        await _client.GetAsync(url);
        stopWatch.Stop();

        return stopWatch.ElapsedMilliseconds;
    }

    public virtual async Task<string> DownloadFileAsync(string fileUrl)
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