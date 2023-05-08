using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.API.Extensions;
using WebsitePerformanceEvaluator.Web.Core.Services;
using WebsitePerformanceEvaluator.Web.Core.ViewModels;

namespace WebsitePerformanceEvaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LinkPerformanceController : Controller
{
    private readonly LinkPerformanceService _linkPerformanceService;

    public LinkPerformanceController(LinkPerformanceService linkPerformanceService)
    {
        _linkPerformanceService = linkPerformanceService;
    }

    /// <summary>
    /// Get a list of link performances by link id.
    /// </summary>
    /// <param name="linkId">The link id.</param>
    /// <returns> List of link performances.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LinkPerformanceViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> LinkPerformances(int linkId)
    {
        var result = await _linkPerformanceService.GetLinkPerformancesAsync(linkId);

        return result.ToOkResult();
    }
}