using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using WebsitePerformanceEvaluator.Core.Extensions;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;

namespace WebsitePerformanceEvaluator.Core.Services;

public class ClientService : IClientService
{
    HttpClient httpClient = new();
    public async Task<IEnumerable<string>> CrawlWebsiteToFindLinks(string url)
    {
        var links = new HashSet<string> { url };
        var visitedLinks = new List<string>();
        var linksToVisit = new List<string> { url };

        while (linksToVisit.Count > 0)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < linksToVisit.Count &&  i < Environment.ProcessorCount * 20; i++)
            {
                var link = linksToVisit[i];

                linksToVisit.RemoveAt(i);
                visitedLinks.Add(link);
                var task = Task.Run(async () =>
                {
                    var newLinks = (await CrawlPageToFindLinks(link)).ApplyFilters(url).ToList();
                    foreach (var item in newLinks)
                    {
                        links.Add(item);
                    }

                    foreach (var newLink in newLinks
                                 .Where(newLink => 
                                     !visitedLinks.Contains(newLink) && !linksToVisit.Contains(newLink)))
                    {
                        linksToVisit.Add(newLink);
                    }

                    return Task.CompletedTask;
                });
                tasks.Add(task);
            }
            
            await Task.WhenAll(tasks);
        }
        return links;
    }

    public async Task<IEnumerable<string>> CrawlPageToFindLinks(string url)
    {
        var doc = await GetDocument(url);

        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (linkNodes == null)
        {
            return new List<string>();
        }

        return linkNodes.Select(link =>
            link.Attributes["href"].Value);
    }

    private async Task<HtmlDocument> GetDocument(string url)
    {
        var doc = new HtmlDocument();

        using var response = httpClient.GetAsync(url).Result;
        var html = response.Content.ReadAsStringAsync().Result;

        doc.LoadHtml(html);

        return doc;
    }

    public int GetTimeResponse(string url)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        var timer = new Stopwatch();

        timer.Start();
        try
        {
            var response = (HttpWebResponse)request.GetResponse();
            response.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while getting response: " + e.Message);
        }

        timer.Stop();

        var timeTaken = timer.Elapsed;

        return timeTaken.Milliseconds;
    }
}