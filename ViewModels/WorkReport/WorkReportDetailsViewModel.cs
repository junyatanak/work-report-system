namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportDetailsViewModel
{
    public int Id { get; init; }
    public string ReporterName { get; init; } = string.Empty;
    public DateOnly WorkDate { get; init; }

    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int OrderQty { get; init; }
    public DateOnly DueDate { get; init; }

    public string WorkClassName { get; init; } = string.Empty;
    public string ProcessName { get; init; } = string.Empty;
    public string WorkPatternName { get; init; } = string.Empty;

    public List<WorkReportWorkerDetailsViewModel> Workers { get; init; } = new();
}