using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class LinkPerformanceController : Controller
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;

    public LinkPerformanceController(ILinkPerformanceRepository linkPerformanceRepository)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
    }
    
    [HttpGet]
    public IActionResult Index(int linkId)
    {
        var linkPerformances = _linkPerformanceRepository.GetByLinkId(linkId);
        
        return View(linkPerformances);
    }
    
}