using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Core.Validators;
using WebsitePerformanceEvaluator.Data;

namespace WebsitePerformanceEvaluator.InfrastructureIoC;

public static class DependencyContainer
{
    public static IServiceCollection ConfigureWebServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(sp => 
            sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger("DefaultLogger"));
       
        return services;
    }
    
    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<DatabaseContext, WebsitePerformanceEvaluatorDatabaseContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

        return services;
    }
    
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
        
        services.AddTransient<LinkService>();
        services.AddTransient<LinkPerformanceService>();

        return services;
    }
    public static IServiceCollection ConfigureConsoleServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();
        services.AddLogging(builder => builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));
        
        return services;
    }
}