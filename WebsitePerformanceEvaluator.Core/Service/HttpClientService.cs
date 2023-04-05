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

    public async Task<HtmlDocument> GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = await Client.GetAsync(url);
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
            await Client.GetAsync(url);
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
        try
        {
            return await Client.GetStringAsync(fileUrl);
        }
        catch (Exception)
        {
            Logger.Error("Error while downloading file");
            return string.Empty;
        }
    }
}