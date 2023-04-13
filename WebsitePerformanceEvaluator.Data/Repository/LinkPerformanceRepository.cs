using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkPerformanceRepository
{
    private readonly WebsitePerformanceEvaluatorContext _repositoryContext;

    protected LinkPerformanceRepository(WebsitePerformanceEvaluatorContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task<IEnumerable<LinkPerformance>> AddRangeAsync(IEnumerable<LinkPerformance> linkPerformance)
    {
        if (linkPerformance == null)
        {
            throw new ArgumentNullException($"{nameof(AddRangeAsync)} entity must not be null");
        }

        try
        {
            await _repositoryContext.AddRangeAsync(linkPerformance);
            await _repositoryContext.SaveChangesAsync();

            return linkPerformance;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(linkPerformance)} could not be saved: {ex.Message}");
        }
    }
}