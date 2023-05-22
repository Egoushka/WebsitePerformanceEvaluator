using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Domain.ViewModels;

namespace WebsitePerformanceEvaluator.Core.Service;

public class LinkPerformanceService
{
    private readonly DatabaseContext _context;

    public LinkPerformanceService(DatabaseContext context)
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
            LinkPerformances = linkPerformances,
        };
    }
}