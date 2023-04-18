using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebsitePerformanceEvaluator.MVC.Extensions;

public static class MatchExtensions
{
    public static IActionResult ToOkOrRedirectResult<TResult, TContract>(
        this Result<TResult> match,
        ITempDataDictionary dataDictionary,
        Func<TResult, TContract> mapper,
        string redirectActionName,
        string redirectControllerName = null)
    {
        return match.Match<IActionResult>(
            Succ: obj =>
            {
                return new RedirectToActionResult(redirectActionName, redirectControllerName, null);
            },
            Fail: error =>
            {
                dataDictionary["Error"] = error.Message;
                    
                return new RedirectToActionResult(redirectActionName, redirectControllerName, null);
            });
    }
}