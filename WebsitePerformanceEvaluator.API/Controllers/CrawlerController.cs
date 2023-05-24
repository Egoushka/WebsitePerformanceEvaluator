using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.API.Extensions;
using WebsitePerformanceEvaluator.API.Requests;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.ViewModels;

namespace WebsitePerformanceEvaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CrawlerController : Controller
{
    private readonly ILinkService _linkService;
    private readonly ILinkPerformanceService _linkPerformanceService;
    
    public CrawlerController(ILinkService linkService, ILinkPerformanceService linkPerformanceService)
    {
        _linkService = linkService;
        _linkPerformanceService = linkPerformanceService;
    }
    
    /// <summary>
    /// Get a paginated list of links.
    /// </summary>
    /// 
    /// <param name="page">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of links per page (default: 7).</param>
    /// <returns>A paginated list of links.</returns>
    [HttpGet("links&page={page}&pageSize={pageSize}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Links(int page = 1, int pageSize = 7)
    {
        var result = await _linkService.GetLinksAsync(page, pageSize);
        
        return result.ToOkResult();
    }
    
    /// <summary>
    /// Get a list of link performances by link id.
    /// </summary>
    /// <param name="linkId">The link id.</param>
    /// <returns> List of link performances.</returns>
    [HttpGet("links/{linkId}/performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkPerformanceViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> LinkPerformances(int linkId)
    {
        var result = await _linkPerformanceService.GetLinkPerformancesAsync(linkId);

        return result.ToOkResult();
    }
    
    /// <summary>
    /// Crawl website and get links performances from it.
    /// </summary>
    /// <returns>Link with list of link performances.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrawlLinkViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CrawlAndRetrieveLink([FromBody] CrawlAndRetrieveLinkRequest request)
    {
        var result = await _linkService.CrawlUrlAsync(request.Url);
        
        return result.ToOkResult();
    }
}