using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ILogger = WebsitePerformanceEvaluator.Infrustructure.Interfaces.ILogger;

namespace WebsitePerformanceEvaluator.MVC;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
        logger.Error("Unhandled exception occurred." + Environment.NewLine + context.Exception);

        context.Result = new RedirectToActionResult("Index", "Error", null);
        context.ExceptionHandled = true;
    }
}
