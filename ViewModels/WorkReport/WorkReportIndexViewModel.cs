namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportIndexViewModel
{
    public WorkReportIndexFilterViewModel Filter { get; set; } = new();
    public List<WorkReportIndexItemViewModel> Items { get; set; } = new();
}