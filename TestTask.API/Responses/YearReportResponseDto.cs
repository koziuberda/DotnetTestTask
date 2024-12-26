namespace TestTask.API.Responses;

public record YearReportResponseDto(int Year, List<YearReportItemResponseDto> Items);