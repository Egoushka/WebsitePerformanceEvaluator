using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Core.Validators;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Core;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddTransient<Crawler>();
        services.AddTransient<WebsiteCrawler>();
        services.AddTransient<SitemapCrawler>();
        services.AddTransient<HttpClientService>();
        services.AddTransient<HtmlParser>();
        services.AddTransient<XmlParser>();
        services.AddTransient<LinkFilter>();
        services.AddTransient<LinkValidator>();
        services.AddTransient<LinkHelper>();

        return services;
    }
}