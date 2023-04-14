using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Repository;

namespace WebsitePerformanceEvaluator.Data;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WebsitePerformanceEvaluatorContext>(options => options.UseSqlServer(connectionString));
        
        services.AddScoped<ILinkPerformanceRepository, LinkPerformanceRepository>();

        return services;
    }
}