using Microsoft.AspNetCore.Mvc;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Models.Enums;

namespace WebsitePerformanceEvaluator.MVC.Controllers
{
    public class LinksController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly ILinkPerformanceRepository _linkPerformanceRepository;
        private readonly Crawler _crawler;

        public LinksController(ILinkRepository linkRepository, ILinkPerformanceRepository linkPerformanceRepository,
            Crawler crawler)
        {
            _linkRepository = linkRepository;
            _linkPerformanceRepository = linkPerformanceRepository;
            _crawler = crawler;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var links = _linkRepository.GetAll();

            return View(links);
        }

        [HttpPost]
        public async Task<IActionResult> GetLinksFromUrl(string url)
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

            await _linkRepository.AddAsync(link);
            await _linkPerformanceRepository.AddRangeAsync(linkPerformances);

            return Index();
        }

        public IActionResult LinkPerformance(int linkId)
        {
            return RedirectToAction("Index", "LinkPerformance", new {linkId});
        }
    }
}
