using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.Web.Core.ViewModels;

namespace WebsitePerformanceEvaluator.Web.Core.Services;

public class LinkPerformanceService
{
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;

    public LinkPerformanceService(WebsitePerformanceEvaluatorDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<LinkPerformanceViewModel>> GetLinkPerformancesAsync(int linkId, string url)
    {
        var link = new Link
        {
            Id = linkId,
            Url = url,
        };

        var linkPerformances = await _context.LinkPerformances
            .Where(item => item.LinkId == linkId)
            .ToListAsync();
        
        return new LinkPerformanceViewModel
        {
            Link = link,
            LinkPerformances = linkPerformances,
        };
    }
}