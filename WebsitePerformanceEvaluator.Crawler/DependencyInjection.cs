using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces.Crawlers;
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
        services.AddTransient<ILinkValidator, LinkValidator>();

        services.AddTransient<ICrawler, CombinedCrawler>();

        services.AddTransient<WebsiteCrawler>();
        services.AddTransient<SitemapCrawler>();
        services.AddTransient<HttpClientService>();
        services.AddTransient<HtmlParser>();
        services.AddTransient<XmlParser>();
        services.AddTransient<LinkFilter>();
        services.AddTransient<LinkHelper>();
        
        return services;
    }
}