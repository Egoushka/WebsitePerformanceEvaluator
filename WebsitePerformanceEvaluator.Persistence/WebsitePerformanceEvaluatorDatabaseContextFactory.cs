using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebsitePerformanceEvaluator.Data;

public class WebsitePerformanceEvaluatorDatabaseContextFactory : IDesignTimeDbContextFactory<WebsitePerformanceEvaluatorDatabaseContext>
{
    public WebsitePerformanceEvaluatorDatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Database=WebsitePerformanceEvaluator;User ID=SA;Password=MyPass@word;TrustServerCertificate=True;";
        
        var optionsBuilder = new DbContextOptionsBuilder<WebsitePerformanceEvaluatorDatabaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new WebsitePerformanceEvaluatorDatabaseContext(optionsBuilder.Options);
    }
}