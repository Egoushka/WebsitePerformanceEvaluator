using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.MVC.Core.Services;
using WebsitePerformanceEvaluator.MVC.Extensions;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkController : Controller
{
    private readonly LinkService _linkService;

    public LinkController(LinkService linkService, ILogger logger)
    {
        _linkService = linkService;
        logger.Log(LogLevel.Information, "LinkController created");
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 7)
    {
        var result = await _linkService.GetLinksAsync(page, pageSize);
        
        return result.ToViewResult(TempData, ViewData ,"Index");
    }

    [HttpPost]
    public async Task<IActionResult> CrawlUrl(string url)
    {
        var result = await _linkService.CrawlUrlAsync(url);
        
        TempData["Url"] = url;
        
        return result.ToRedirectResult(TempData, "Index", "Link");
    }
}

