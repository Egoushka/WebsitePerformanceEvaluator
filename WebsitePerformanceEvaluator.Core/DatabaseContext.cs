using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Domain.Models;

namespace WebsitePerformanceEvaluator.Core;

public abstract class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<LinkPerformance> LinkPerformances { get; set; }
    public DbSet<Link> Links { get; set; }
}