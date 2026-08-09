namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportWorkerInputViewModel
{
    public int? WorkerNumber { get; set; }
    public int? WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public TimeOnly StartAt { get; set; }
    public TimeOnly EndAt { get; set; }
    public int BreakMinutes { get; set; } = 0;
    public int? ProducedQty { get; set; }
}