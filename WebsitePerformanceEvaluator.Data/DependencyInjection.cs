using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebsitePerformanceEvaluator.Data;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WebsitePerformanceEvaluatorDatabaseContext>(options => options.UseSqlServer(connectionString));
        
        return services;
    }
}