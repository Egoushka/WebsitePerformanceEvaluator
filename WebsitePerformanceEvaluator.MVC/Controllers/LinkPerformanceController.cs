using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.MVC.ViewModels;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkPerformanceController : Controller
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;

    public LinkPerformanceController(ILinkPerformanceRepository linkPerformanceRepository)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
    }
    
    [HttpGet]
    public IActionResult Index(int linkId, string url)
    {
        var link = new Link
        {
            Id = linkId,
            Url = url,
        };
        
        var linkPerformances = _linkPerformanceRepository.GetByLinkId(linkId);
        
        var viewModel = new LinkPerformanceViewModel
        {
            Link = link,
            LinkPerformances = linkPerformances,
        };

        return View(viewModel);
    }
    [HttpGet]
    public IActionResult BackToLinks()
    {
        return RedirectToAction("Index", "Links");
    }
    
}