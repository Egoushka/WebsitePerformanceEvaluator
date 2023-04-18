using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Data.Repository;
using WebsitePerformanceEvaluator.MVC.Core.ViewModels;

namespace WebsitePerformanceEvaluator.MVC.Core.Services;

public class LinkPerformanceService
{
    private readonly LinkPerformanceRepository _linkPerformanceRepository;

    public LinkPerformanceService(LinkPerformanceRepository linkPerformanceRepository)
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