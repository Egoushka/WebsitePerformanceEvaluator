using System.Diagnostics;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

namespace WebsitePerformanceEvaluator.Console;

public class TaskRunner
{
    private readonly ConsoleHelper _consoleHelper;
    private readonly ConsoleWrapper _consoleWrapper;
    private readonly Crawler _crawler;
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;
    private readonly ILinkRepository _linkRepository;

    public TaskRunner(Crawler crawler, ConsoleWrapper consoleWrapper, ConsoleHelper consoleHelper,
        ILinkPerformanceRepository linkPerformanceRepository, ILinkRepository linkRepository)
    {
        _crawler = crawler;
        _consoleWrapper = consoleWrapper;
        _consoleHelper = consoleHelper;
        _linkPerformanceRepository = linkPerformanceRepository;
        _linkRepository = linkRepository;
    }

    public async Task RunAsync()
    {
        var watch = Stopwatch.StartNew();

        _consoleWrapper.WriteLine("Enter website url:");
        var url = _consoleWrapper.ReadLine();

        watch.Start();

        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);
        
        await SaveLinksToDatabaseAsync(links, url);

        PrintLinksInCrawlingNotInSitemap(links);
        PrintLinksInSitemapNotInCrawling(links);

        PrintLinksWithTimeResponse(links);

        PrintAmountOfFoundLinks(links);

        watch.Stop();
        _consoleWrapper.WriteLine($"Time elapsed: {watch.ElapsedMilliseconds / 1000} s");
    }

    private void PrintLinksInCrawlingNotInSitemap(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInCrawlingNotInSitemap = linkPerformances
            .Where(x => x.CrawlingLinkSource == CrawlingLinkSource.Website)
            .Select(x => x.Url);

        _consoleWrapper.WriteLine("Links found after crawling website, but not in sitemap:");

        if (linksInCrawlingNotInSitemap.Any())
        {
            _consoleHelper.PrintTable(new List<string> { "Link" }, linksInCrawlingNotInSitemap);
        }
        else
        {
            _consoleWrapper.WriteLine("No links found");
        }

        _consoleWrapper.WriteLine();
    }

    private void PrintLinksInSitemapNotInCrawling(IEnumerable<LinkPerformance> linkPerformances)
    {
        var linksInSitemapNotInCrawling = linkPerformances
            .Where(link => link.CrawlingLinkSource == CrawlingLinkSource.Sitemap)
            .Select(link => link.Url);

        _consoleWrapper.WriteLine("Links in sitemap, that wasn't found after crawling:");

        if (linksInSitemapNotInCrawling.Any())
        {
            _consoleHelper.PrintTable(new List<string> { "Link" }, linksInSitemapNotInCrawling);
        }
        else
        {
            _consoleWrapper.WriteLine("No links found");
        }

        _consoleWrapper.WriteLine();
    }

    private void PrintLinksWithTimeResponse(IEnumerable<LinkPerformance> linkPerformances)
    {
        var rowsList = linkPerformances
            .OrderBy(item => item.TimeResponseMs)
            .Select(x => new Tuple<string, long?>(x.Url, x.TimeResponseMs));

        _consoleWrapper.WriteLine("Links with time response:");

        _consoleHelper.PrintTable(new List<string> { "Link", "Time(ms)" }, rowsList);

        _consoleWrapper.WriteLine();
    }

    private void PrintAmountOfFoundLinks(IEnumerable<LinkPerformance> links)
    {
        var countInBoth = links.Count(l => l.CrawlingLinkSource == CrawlingLinkSource.WebsiteAndSitemap);

        var sitemapLinksCount = countInBoth + links.Count(l => l.CrawlingLinkSource == CrawlingLinkSource.Sitemap);
        var crawlingLinksCount = countInBoth + links.Count(l => l.CrawlingLinkSource == CrawlingLinkSource.Website);

        _consoleWrapper.WriteLine($"Links in sitemap: {sitemapLinksCount}");
        _consoleWrapper.WriteLine($"Links after crawling: {crawlingLinksCount}");

        _consoleWrapper.WriteLine();
    }
    private async Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url)
    {
        var link = new Data.Models.Link
        {
            Url = url,
        };

        var linksData = links.Select(x => new WebsitePerformanceEvaluator.Data.Models.LinkPerformance
        {
            Url = x.Url,
            TimeResponseMs = x.TimeResponseMs,
            CrawlingLinkSource = (Data.Models.Enums.CrawlingLinkSource)x.CrawlingLinkSource,
            Link = link,
        });
        
        await _linkRepository.AddAsync(link);
        await _linkPerformanceRepository.AddRangeAsync(linksData);
    }
}