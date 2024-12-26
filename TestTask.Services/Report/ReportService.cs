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
        var reportItems = ReportQueryHelper.GetMostPopularItemsReport(_testDbContext);
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