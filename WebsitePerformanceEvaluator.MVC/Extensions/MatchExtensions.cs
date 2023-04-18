using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebsitePerformanceEvaluator.MVC.Extensions;

public static class MatchExtensions
{
    public static IActionResult ToRedirectResult<TResult>(
        this Result<TResult> match,
        ITempDataDictionary dataDictionary,
        string redirectActionName,
        string redirectControllerName = null)
    {
        return match.Match<IActionResult>(
            Succ: _ =>
            {
                return new RedirectToActionResult(redirectActionName, redirectControllerName, null);
            },
            Fail: error =>
            {
                dataDictionary["Error"] = error.Message;
                    
                return new RedirectToActionResult("Index", "Error", null);
            });
    }
    
    public static IActionResult ToViewResult<TResult>(
        this Result<TResult> match,
        ITempDataDictionary dataDictionary,
        ViewDataDictionary viewDataDictionary,
        string viewName)
    {
        return match.Match<IActionResult>(
            Succ: result =>
            {
                viewDataDictionary.Model = result;
                    
                return new ViewResult
                {
                    ViewName = viewName,
                    ViewData = viewDataDictionary,
                };
            },
            Fail: error =>
            {
                dataDictionary["Error"] = error.Message;

                return new RedirectToActionResult("Index", "Error", null);
            });
    }
}

