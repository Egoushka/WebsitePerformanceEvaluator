using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebsitePerformanceEvaluator.API.Extensions;

public static class MatchExtensions
{
    public static IActionResult ToOkResult<TResult>(
        this Result<TResult> result)
    {
        return result.Match<IActionResult>(
            Succ: value => new OkObjectResult(value),
            Fail: error => new BadRequestObjectResult(error));
    }
}

