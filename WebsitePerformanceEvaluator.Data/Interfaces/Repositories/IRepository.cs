namespace WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
}