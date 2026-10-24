using DailyWorkReport.ViewModels.Paging;

namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportIndexViewModel : PagedViewModel<WorkReportIndexItemViewModel>
{
    public WorkReportIndexFilterViewModel Filter { get; set; } = new();
}