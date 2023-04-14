using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Models.Enums;

namespace WebsitePerformanceEvaluator.MVC.Controllers
{
    public class LinksController : Controller
    {
        private readonly ILinkPerformanceRepository _linkPerformanceRepository;
        private readonly Crawler _crawler;

        public LinksController(ILinkPerformanceRepository linkPerformanceRepository, Crawler crawler)
        {
            _linkPerformanceRepository = linkPerformanceRepository;
            _crawler = crawler;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string url)
        {
            var links = await _crawler.CrawlWebsiteAndSitemapAsync(url);

            var link = new Link
            {
                Url = url
            };

            var linkPerformances = links.Select(x => new LinkPerformance
            {
                Url = x.Url,
                CrawlingLinkSource = (CrawlingLinkSource)x.CrawlingLinkSource,
                TimeResponseMs = x.TimeResponseMs,
                Link = link,
            });

            return View(linkPerformances);
        }
    }
}
