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

    public LinkPerformanceViewModel GetLinkPerformances(int linkId, string url)
    {
        var link = new Link
        {
            Id = linkId,
            Url = url,
        };
        
        var linkPerformances = _linkPerformanceRepository.GetByLinkId(linkId);
        
        var viewModel = new LinkPerformanceViewModel
        {
            Link = link,
            LinkPerformances = linkPerformances,
        };
        
        return viewModel;
    }
}