using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Infrastructure.Interfaces;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkBaseRepository: BaseRepository<Link>, ILinkRepository
{
    public LinkBaseRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext, ILogger logger)
        : base(repositoryDatabaseContext, logger)
    {
    }
}