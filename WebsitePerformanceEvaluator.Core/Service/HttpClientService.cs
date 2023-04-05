using System.Diagnostics;
using HtmlAgilityPack;
using Serilog;

namespace WebsitePerformanceEvaluator.Core.Service;

public class HttpClientService
{
    private HttpClient Client { get; }
    private ILogger Logger { get; }

    public HttpClientService(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        Client = httpClientFactory.CreateClient();
        Logger = logger;
    }

    public HtmlDocument GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = Client.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return doc;
    }

    public int GetTimeResponse(string url)
    {
        var time = 0;
        try
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            Client.GetAsync(url).Wait();
            stopWatch.Stop();

            time = (int)stopWatch.ElapsedMilliseconds;
        }
        catch (Exception)
        {
            Logger.Error("Error while getting response time");
        }

        return time;
    }

    public async Task<string> DownloadFile(string fileUrl)
    {
        string sitemapString;
        try
        {
            sitemapString = await Client.GetStringAsync(fileUrl);
        }
        catch (Exception)
        {
            Logger.Error("Error while downloading file");
            return "";
        }

        return sitemapString;
    }
}