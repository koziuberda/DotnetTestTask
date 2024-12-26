using TestTask.API.Responses;
using TestTask.Services.Report.ResponseModels;

namespace TestTask.API.Mapping;

public static class ReportMapper
{
    public static YearReportResponseDto MapToReport(YearReport report) => 
        new YearReportResponseDto(report.Year, report.Items.Select(MapToReportItem).ToList());

    private static YearReportItemResponseDto MapToReportItem(YearReportItem item) =>
        new YearReportItemResponseDto(item.ItemName, item.MaxPurchasesInOneDayPerUser);
}