using Microsoft.AspNetCore.Mvc;
using TestTask.API.Mapping;
using TestTask.API.Responses;
using TestTask.Services.Report;

namespace TestTask.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("report")]
    public async Task<PopularItemsResponseDto> GetPopularItemsAsync()
    {
        var yearReports = await _reportService.GetPopularItemsAsync();
        var yearReportsDto = yearReports.Select(ReportMapper.MapToReport).ToList();
        
        return new PopularItemsResponseDto(yearReportsDto);
    }
}