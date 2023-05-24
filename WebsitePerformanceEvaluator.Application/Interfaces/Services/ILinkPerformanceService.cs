using LanguageExt.Common;
using WebsitePerformanceEvaluator.Core.ViewModels;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ILinkPerformanceService
{
    public Task<Result<LinkPerformanceViewModel>> GetLinkPerformancesAsync(int linkId);
}