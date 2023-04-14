using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceBaseRepository : BaseRepository<LinkPerformance>, ILinkPerformanceRepository
{
    public LinkPerformanceBaseRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext) : base(repositoryDatabaseContext)
    {
    }
}