using LanguageExt.Common;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.MVC.Core.ViewModels;

namespace WebsitePerformanceEvaluator.MVC.Core.Services;

public class LinkPerformanceService
{
    private readonly ILinkPerformanceRepository _linkPerformanceRepository;

    public LinkPerformanceService(ILinkPerformanceRepository linkPerformanceRepository)
    {
        _linkPerformanceRepository = linkPerformanceRepository;
    }

    public async Task<Result<LinkPerformanceViewModel>> GetLinkPerformancesAsync(int linkId, string url)
    {
        var link = new Link
        {
            Id = linkId,
            Url = url,
        };

        var linkPerformances = await _linkPerformanceRepository.GetByLinkIdAsync(linkId);

        return new LinkPerformanceViewModel
        {
            Link = link,
            LinkPerformances = linkPerformances,
        };
    }
}