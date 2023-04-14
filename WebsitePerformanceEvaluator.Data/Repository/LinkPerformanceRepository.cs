using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceRepository : Repository<LinkPerformance>, ILinkPerformanceRepository
{
    public LinkPerformanceRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext) : base(repositoryDatabaseContext)
    {
    }
}