namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportIndexItemViewModel
{
    public int Id { get; init; }
    public DateOnly WorkDate { get; init; }
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string ProcessName { get; init; } = string.Empty;
    public int TotalProducedQty { get; init; }
    public string ReporterName { get; init; } = string.Empty;
    public bool CanEdit { get; init; }
}