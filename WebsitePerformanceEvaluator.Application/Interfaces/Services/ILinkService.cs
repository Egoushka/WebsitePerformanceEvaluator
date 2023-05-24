using LanguageExt.Common;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.ViewModels;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ILinkService
{
    public Task<Result<LinkViewModel>> GetLinksAsync(int page, int pageSize);
    
    public Task<Result<CrawlLinkViewModel>> CrawlUrlAsync(string url);

    public Task SaveLinksToDatabaseAsync(IEnumerable<LinkPerformance> links, string url);
}
