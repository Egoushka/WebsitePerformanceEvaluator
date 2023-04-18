using Microsoft.AspNetCore.Mvc;

namespace WebsitePerformanceEvaluator.MVC.Controllers;

public class ErrorController: Controller
{
    public ActionResult Index()
    {
        return View();
    }
}