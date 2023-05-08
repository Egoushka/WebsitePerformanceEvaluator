namespace WebsitePerformanceEvaluator.API;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureAPIServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(sp => 
            sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DefaultLogger"));
        
        return services;
    }
}