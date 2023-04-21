using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkBaseRepository: BaseRepository<Link>, ILinkRepository
{
    public LinkBaseRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext, ILogger logger)
        : base(repositoryDatabaseContext, logger)
    {
    }
}