namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportIndexFilterViewModel
{
    public DateOnly? WorkDateFrom { get; set; }
    public DateOnly? WorkDateTo { get; set; }
    public string? ProductionOrderNumber { get; set; } 
    public string? ProductName { get; set; }
    public string? ProcessName { get; set; }
    public string? ReporterName { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}