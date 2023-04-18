using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Infrustructure.Interfaces;

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