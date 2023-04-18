using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Infrastructure.Interfaces;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceBaseRepository : BaseRepository<LinkPerformance>, ILinkPerformanceRepository
{
    public LinkPerformanceBaseRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext, ILogger logger)
        : base(repositoryDatabaseContext, logger)
    {
    }

    public IEnumerable<LinkPerformance> GetByLinkId(int linkId)
    {
        return _repositoryDatabaseContext.LinkPerformances.Where(x => x.LinkId == linkId);
    }
}