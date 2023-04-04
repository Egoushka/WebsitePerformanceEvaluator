using System.Diagnostics;
using HtmlAgilityPack;
using Serilog;

namespace WebsitePerformanceEvaluator.Core.Service;

public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    
    public HttpClientService(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public HtmlDocument GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = _httpClient.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return doc;
    }

    public  int GetTimeResponse(string url)
    {
        var time = 0;
        try
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            _httpClient.GetAsync(url).Wait();
            stopWatch.Stop();

            time = (int)stopWatch.ElapsedMilliseconds;
        }
        catch (Exception)
        {
            _logger.Error("Error while getting response time");
        }

        return time;
    }
    public async Task<string> DownloadSitemap(string sitemapUrl)
    {
        string sitemapString;
        try
        {
            sitemapString = await _httpClient.GetStringAsync(sitemapUrl);
        }
        catch (Exception e)
        {
            _logger.Error("Error while downloading sitemap, sitemap will be ignored");
            return "";
        }

        return sitemapString;
    }
}