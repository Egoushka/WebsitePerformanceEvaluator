using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.MVC.Core.Services;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkController : Controller
{
    private readonly LinkService _linkService;

    public LinkController(LinkService linkService)
    {
        _linkService = linkService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 7)
    {
        var viewModel = await _linkService.GetLinksAsync(page, pageSize);
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GetLinksFromUrl(string url)
    {
        await _linkService.GetLinksFromUrlAsync(url);
        
        return RedirectToAction("Index");
    }
}

