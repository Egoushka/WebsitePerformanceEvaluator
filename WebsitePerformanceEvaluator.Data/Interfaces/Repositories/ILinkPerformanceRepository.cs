using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Interfaces.Repositories;

public interface ILinkPerformanceRepository : IRepository<LinkPerformance>
{
    IEnumerable<LinkPerformance> GetByLinkId(int linkId);
}