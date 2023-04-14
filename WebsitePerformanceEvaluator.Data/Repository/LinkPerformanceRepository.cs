using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceRepository : ILinkPerformanceRepository
{
    private readonly WebsitePerformanceEvaluatorDatabaseContext _repositoryDatabaseContext;

    public LinkPerformanceRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext)
    {
        _repositoryDatabaseContext = repositoryDatabaseContext;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> AddRangeAsync(IEnumerable<LinkPerformance> linkPerformance)
    {
        if (linkPerformance == null)
        {
            throw new ArgumentNullException($"Entity must not be null");
        }

        try
        {
            await _repositoryDatabaseContext.AddRangeAsync(linkPerformance);
            await _repositoryDatabaseContext.SaveChangesAsync();

            return linkPerformance;
        }
        catch (Exception ex)
        {
            throw new Exception($"Links could not be saved: {ex.Message}");
        }
    }
}