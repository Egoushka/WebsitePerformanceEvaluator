using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.MVC.Core.Services;
using WebsitePerformanceEvaluator.MVC.Extensions;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkPerformanceController : Controller
{
    private readonly LinkPerformanceService _linkPerformanceService;

    public LinkPerformanceController(LinkPerformanceService linkPerformanceService)
    {
        _linkPerformanceService = linkPerformanceService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int linkId, string url)
    {
        var result = await _linkPerformanceService.GetLinkPerformancesAsync(linkId, url);

        return result.ToViewResult(TempData, ViewData,"Index");
    }
}