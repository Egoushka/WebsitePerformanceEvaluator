using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.MVC.Extensions;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkPerformanceController : Controller
{
    private readonly ILinkPerformanceService _linkPerformanceService;

    public LinkPerformanceController(ILinkPerformanceService linkPerformanceService)
    {
        _linkPerformanceService = linkPerformanceService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int linkId)
    {
        var result = await _linkPerformanceService.GetLinkPerformancesAsync(linkId);

        return result.ToViewResult(TempData, ViewData,"Index");
    }
}