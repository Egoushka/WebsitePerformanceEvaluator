using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Data.Models;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Core.Validators;

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

        services.AddDbContext<WebsitePerformanceEvaluatorContext>(options =>
            options.UseSqlServer(
                "Server=localhost;Database=WebsitePerformanceEvaluator;User ID=SA;Password=MyPass@word;TrustServerCertificate=True;"));

        return services;
    }
}