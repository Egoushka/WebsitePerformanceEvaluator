using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.ViewModels;
using WebsitePerformanceEvaluator.Data;

namespace WebsitePerformanceEvaluator.Core.Services;

public class LinkPerformanceService : ILinkPerformanceService
{
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;

    public LinkPerformanceService(WebsitePerformanceEvaluatorDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<LinkPerformanceViewModel>> GetLinkPerformancesAsync(int linkId)
    {
        var link = await _context.Links
            .FirstOrDefaultAsync(item => item.Id == linkId);
        
        var linkPerformances = await _context.LinkPerformances
            .Where(item => item.LinkId == linkId)
            .ToListAsync();
        
        return new LinkPerformanceViewModel
        {
            Link = link,
            LinkPerformances = linkPerformances
        };
    }
}