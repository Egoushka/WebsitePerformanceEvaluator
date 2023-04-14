using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<IEnumerable<LinkPerformance>> AddRangeAsync(IEnumerable<TEntity> entities);
}