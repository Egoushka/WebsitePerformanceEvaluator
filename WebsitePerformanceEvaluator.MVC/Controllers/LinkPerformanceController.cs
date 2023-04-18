using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.MVC.Core.Services;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkPerformanceController : Controller
{
    private readonly LinkPerformanceService _linkPerformanceService;

    public LinkPerformanceController(LinkPerformanceService linkPerformanceService)
    {
        _linkPerformanceService = linkPerformanceService;
    }
    
    [HttpGet]
    public IActionResult Index(int linkId, string url)
    {
        var viewModel = _linkPerformanceService.GetLinkPerformances(linkId, url);

        return View(viewModel);
    }
}