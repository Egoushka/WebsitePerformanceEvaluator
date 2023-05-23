using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces.Crawlers;
using WebsitePerformanceEvaluator.Core.Interfaces.FIlters;
using WebsitePerformanceEvaluator.Core.Interfaces.Helpers;
using WebsitePerformanceEvaluator.Core.Interfaces.Parsers;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Interfaces.Validators;
using WebsitePerformanceEvaluator.Crawler.Crawlers;
using WebsitePerformanceEvaluator.Crawler.Filters;
using WebsitePerformanceEvaluator.Crawler.Helpers;
using WebsitePerformanceEvaluator.Crawler.Parsers;
using WebsitePerformanceEvaluator.Crawler.Services;
using WebsitePerformanceEvaluator.Crawler.Validators;

namespace WebsitePerformanceEvaluator.Crawler;

public static class DependencyInjection
{
    public static IServiceCollection AddCrawlerServices(this IServiceCollection services)
    {
        services.AddTransient<ICombinedCrawler, CombinedCrawler>();
        services.AddTransient<IWebsiteCrawler, WebsiteCrawler>();
        services.AddTransient<ISitemapCrawler, SitemapCrawler>();
        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddTransient<IHtmlParser, HtmlParser>();
        services.AddTransient<IXmlParser, XmlParser>();
        services.AddTransient<ILinkFilter, LinkFilter>();
        services.AddTransient<ILinkValidator, LinkValidator>();
        services.AddTransient<ILinkHelper, LinkHelper>();
        
        return services;
    }
    
}