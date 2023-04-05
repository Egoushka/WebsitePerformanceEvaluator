using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator;

internal static class Program
{
    private static IContainer CompositionRoot()
    {
        var services = new ServiceCollection();

        services.AddMemoryCache();
        services.AddOptions();
        services.Configure<MemoryCacheEntryOptions>(options => options.SetSlidingExpiration(TimeSpan.FromMinutes(1)));
        
        services.AddHttpClient();
        services.AddTransient<Crawler>();
        services.AddTransient<WebsiteCrawler>();
        services.AddTransient<SitemapCrawler>();
        services.AddTransient<HttpClientService>();
        services.AddTransient<HtmlParser>();
        services.AddTransient<XmlParser>();
        services.AddTransient<LinkFilter>();
        services.AddTransient<Application>();
        services.AddTransient<TaskRunner>();

        var builder = new ContainerBuilder();
        
        builder.Register<ILogger>((c, p) =>
            new LoggerConfiguration().WriteTo.Console().CreateLogger()
            ).SingleInstance();
        
        builder.Populate(services);
        
        return builder.Build();
    }

    public static async Task Main()
    {
        await CompositionRoot().Resolve<Application>().Run();
    }
}