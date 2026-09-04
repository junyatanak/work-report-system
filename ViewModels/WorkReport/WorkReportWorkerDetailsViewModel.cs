namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportWorkerDetailsViewModel
{
    public string WorkerNumber { get; init; } = string.Empty;
    public string WorkerName { get; init; } = string.Empty;

    public DateTime StartAt { get; init; }
    public DateTime EndAt { get; init; }

    public int BreakMinutes { get; init; }
    public int ProducedQty { get; init; }
}