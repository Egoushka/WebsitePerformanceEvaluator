using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

public interface ILinkPerformanceRepository : IRepository<LinkPerformance>
{
    Task<List<LinkPerformance>> GetByLinkIdAsync(int linkId);
}