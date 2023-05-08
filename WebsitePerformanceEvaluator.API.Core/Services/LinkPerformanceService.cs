using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.API.Core.Dto.LinkPerformance;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvaluator.Data.Models;
using WebsitePerformanceEvaluator.MVC.Core.ViewModels;

namespace WebsitePerformanceEvaluator.API.Core.Services;

public class LinkPerformanceService
{
    private readonly WebsitePerformanceEvaluatorDatabaseContext _context;

    public LinkPerformanceService(WebsitePerformanceEvaluatorDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<GetLinkPerformanceDto>>> GetLinkPerformancesAsync(int linkId)
    {
        var linkPerformances = await _context.LinkPerformances
            .Where(item => item.LinkId == linkId)
            .ToListAsync();

        var result= linkPerformances.Select(item => new GetLinkPerformanceDto
        {
            Id = item.Id,
            Url = item.Url,
            TimeResponseMs = item.TimeResponseMs,
            CrawlingLinkSource = (int)item.CrawlingLinkSource
        });
        
        return new Result<IEnumerable<GetLinkPerformanceDto>>(result);
    }
}