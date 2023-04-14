using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data;

public class WebsitePerformanceEvaluatorDatabaseContext : DbContext
{
    public DbSet<LinkPerformance> LinkPerformances { get; set; }
    
    public WebsitePerformanceEvaluatorDatabaseContext(DbContextOptions<WebsitePerformanceEvaluatorDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}