using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceRepository
{
    private readonly WebsitePerformanceEvaluatorContext _repositoryContext;

    public LinkPerformanceRepository(WebsitePerformanceEvaluatorContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public virtual async Task<IEnumerable<LinkPerformance>> AddRangeAsync(IEnumerable<LinkPerformance> linkPerformance)
    {
        if (linkPerformance == null)
        {
            throw new ArgumentNullException($"Entity must not be null");
        }

        try
        {
            await _repositoryContext.AddRangeAsync(linkPerformance);
            await _repositoryContext.SaveChangesAsync();

            return linkPerformance;
        }
        catch (Exception ex)
        {
            throw new Exception($"Links could not be saved: {ex.Message}");
        }
    }
}