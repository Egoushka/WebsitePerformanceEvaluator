using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.API.Extensions;
using WebsitePerformanceEvaluator.Web.Core.Services;
using WebsitePerformanceEvaluator.Web.Core.ViewModels;

namespace WebsitePerformanceEvaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class LinkController : Controller
{
    private readonly LinkService _linkService;
    
    public LinkController(LinkService linkService)
    {
        _linkService = linkService;
    }
    
    /// <summary>
    /// Get a paginated list of links.
    /// </summary>
    /// 
    /// <param name="page">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of links per page (default: 7).</param>
    /// <returns>A paginated list of links.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Links(int page = 1, int pageSize = 7)
    {
        var result = await _linkService.GetLinksAsync(page, pageSize);
        
        return result.ToOkResult();
    }
    
    /// <summary>
    /// Crawl website and get links performances from it.
    /// </summary>
    /// <param name="url">The link url.</param>
    /// <returns>Link with list of link performances.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CrawlUrl(string url)
    {
        var result = await _linkService.CrawlUrlAsync(url);
        
        return result.ToOkResult();
    }
}