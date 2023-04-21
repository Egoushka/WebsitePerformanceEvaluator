using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceRepository : BaseRepository<LinkPerformance>, ILinkPerformanceRepository
{
    public LinkPerformanceRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext, ILogger logger)
        : base(repositoryDatabaseContext, logger)
    {
    }

    public Task<List<LinkPerformance>> GetByLinkIdAsync(int linkId)
    {
        return _repositoryDatabaseContext.LinkPerformances
            .Where(x => x.LinkId == linkId)
            .ToListAsync();
    }
}