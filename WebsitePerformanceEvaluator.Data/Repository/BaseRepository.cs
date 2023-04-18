using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    protected readonly WebsitePerformanceEvaluatorDatabaseContext _repositoryDatabaseContext;

    protected BaseRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext)
    {
        _repositoryDatabaseContext = repositoryDatabaseContext;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
        }

        try
        {
            await _repositoryDatabaseContext.AddAsync(entity);
            await _repositoryDatabaseContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
        }
    }
    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException($"{nameof(AddRangeAsync)} entity must not be null");
        }

        try
        {
            await _repositoryDatabaseContext.AddRangeAsync(entities);
            await _repositoryDatabaseContext.SaveChangesAsync();

            return entities;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entities)} could not be saved: {ex.Message}");
        }
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        return _repositoryDatabaseContext.Set<TEntity>();
    }
}