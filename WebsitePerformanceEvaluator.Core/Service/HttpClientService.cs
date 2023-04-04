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

    public int GetTimeResponse(string url)
    {
        var time = 0;
        try
        {
            var timeAtStart = DateTime.Now;

            var result = _httpClient.GetAsync(url).Result;
            var responseTime = result.Headers.TryGetValues("X-Response-Time", out var values)
                ? values.FirstOrDefault()
                : null;
            time = responseTime == null ? (DateTime.Now - timeAtStart).Milliseconds : int.Parse(responseTime);
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