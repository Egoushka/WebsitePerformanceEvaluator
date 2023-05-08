using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;
using WebsitePerformanceEvaluator.API.Core.Services;
using WebsitePerformanceEvaluator.API.Extensions;

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
    /// Get a paginated list of link performances.
    /// </summary>
    /// <param name="linkId">The link id.</param>
    /// <param name="url">The link url.</param>
    /// <returns> List of link performances.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetLinkPerformanceDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> LinkPerformances(int linkId)
    {
        var result = await _linkPerformanceService.GetLinkPerformancesAsync(linkId);

        return result.ToOkResult();
    }
}