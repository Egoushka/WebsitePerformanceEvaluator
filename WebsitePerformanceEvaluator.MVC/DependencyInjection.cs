namespace WebsitePerformanceEvaluator.MVC;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(sp => 
            sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DefaultLogger"));
        
        return services;
    }
}