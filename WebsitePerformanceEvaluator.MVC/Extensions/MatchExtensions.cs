using System.ComponentModel.DataAnnotations;
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
                dataDictionary["Error"] = GetErrorMessage(error);
                    
                return new RedirectToActionResult(redirectActionName, redirectControllerName, null);
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
                dataDictionary["Error"] = GetErrorMessage(error);

                return new ViewResult
                {
                    ViewName = viewName,
                    ViewData = viewDataDictionary,
                };
            });
    }
    
    private static string GetErrorMessage(Exception e)
    {
        if(e is ValidationException validationException)
        {
            return "Validation error: " + validationException.Message;
        }
        
        return e.Message;
    }
}

