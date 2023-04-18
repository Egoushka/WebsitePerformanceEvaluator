using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.MVC.Services;
using WebsitePerformanceEvaluator.MVC.ViewModels;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkController : Controller
{
    private readonly Crawler _crawler;
    private readonly LinkService _linkService;

    public LinkController(Crawler crawler, LinkService linkService)
    {
        _crawler = crawler;
        _linkService = linkService;
    }

    [HttpGet]
    public IActionResult Index(int page = 1, int pageSize = 7)
    {
        var viewModel = _linkService.GetLinks(page, pageSize);
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GetLinksFromUrl(string url)
    {
        var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        await _linkService.SaveLinksToDatabaseAsync(links, url);
            
        return RedirectToAction("Index");
    }
}

