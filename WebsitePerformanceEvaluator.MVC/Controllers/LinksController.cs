using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.MVC.Services;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinksController : Controller
{
    private readonly Crawler _crawler;
    private readonly LinkService _linkService;

    public LinksController(Crawler crawler, LinkService linkService)
    {
        _crawler = crawler;
        _linkService = linkService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var links = _linkService.GetLinks();

        return View(links);
    }

    [HttpPost]
    public async Task<IActionResult> GetLinksFromUrl(string url)
    {
        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        await _linkService.SaveLinksToDatabaseAsync(links, url);
            
        return Index();
    }

    public IActionResult LinkPerformance(int linkId)
    {
        return RedirectToAction("Index", "LinkPerformance", new {linkId});
    }
}