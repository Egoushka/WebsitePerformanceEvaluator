using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Repository;

public class LinkRepository: Repository<Link>, ILinkRepository
{
    public LinkRepository(WebsitePerformanceEvaluatorDatabaseContext repositoryDatabaseContext) : base(repositoryDatabaseContext)
    {
    }
}