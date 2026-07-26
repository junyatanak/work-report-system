namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportWorkerInputViewModel
{
    public int WorkerId { get; set; }
    public TimeOnly StartAt { get; set; }
    public TimeOnly EndAt { get; set; }
    public int BreakMinutes { get; set; } = 0;
    public int ProducedQty { get; set; }
}