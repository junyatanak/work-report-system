using Microsoft.AspNetCore.Mvc.Rendering;

namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportCreateViewModel
{
    public DateOnly WorkDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int ProductionOrderId { get; set; }
    public int ProcessId { get; set; }
    public int WorkPatternId { get; set; }

    public List<WorkReportWorkerInputViewModel> WorkReportWorkers { get; set; } = new ();

    public List<SelectListItem> ProductionOrderOptions { get; set; } = new ();
    public List<SelectListItem> ProcessOptions { get; set; } = new ();
    public List<SelectListItem> WorkPatternOptions { get; set; } = new ();
    public List<SelectListItem> WorkerOptions { get; set; } = new ();
}