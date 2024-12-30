using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Data.Entities;
using TestTask.Services.Report.InternalModels;
using TestTask.Services.Report.ResponseModels;

namespace TestTask.Services.Report;

public class ReportService
{
    private readonly TestDbContext _testDbContext;

    public ReportService(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<IEnumerable<YearReport>> GetPopularItemsAsync()
    {
        var groupped = await _testDbContext.UserItems
            .GroupBy(ui => new 
            { 
                ui.UserId,
                ui.ItemId,
                Year = ui.PurchaseDate.Year,
                Day = ui.PurchaseDate.Date
            })
            .Select(g => new
            {
                g.Key.ItemId,
                g.Key.Year,
                CountInThatDay = g.Count()
            })
            .GroupBy(x => new { x.ItemId, x.Year })
            .Select(g => new
            {
                ItemId = g.Key.ItemId,
                Year = g.Key.Year,
                MaxPurchasesOneDayOneUser = g.Max(x => x.CountInThatDay)
            })
            .GroupBy(x => x.Year)
            .ToListAsync();
        
        var reportItems = groupped
            .SelectMany(g => 
                g.OrderByDescending(x => x.MaxPurchasesOneDayOneUser) 
                    .Take(3)                                        
            )
            .OrderBy(x => x.Year)
            .ThenByDescending(x => x.MaxPurchasesOneDayOneUser)
            .Select(x => new ReportItem(x.Year, x.ItemId, x.MaxPurchasesOneDayOneUser))
            .ToList();
        
        var itemsIds = reportItems.Select(x => x.ItemId);
        var items = await _testDbContext.Items.Where(x => itemsIds.Contains(x.Id)).ToListAsync();
        
        return MapToYearReports(reportItems, items);
    }

    private List<YearReport> MapToYearReports(
        List<ReportItem>? reportItems, List<Item> items)
    {
        var yearReports = reportItems?
            .GroupBy(i => i.Year)
            .Select(g => new YearReport(
                g.Key,
                g.Select(ri => new YearReportItem(
                    items.Where(i => i.Id == ri.ItemId).Select(x => x.Name).First(),
                    ri.MaxPurchasesOneDayOneUser
                )).ToList()
            ))
            .ToList();

        return yearReports ?? [];
    }
}