using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DailyWorkReport.ViewModels.WorkReport;

public class WorkReportEditViewModel
{
    public int Id { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Work date is required.")]
    public DateOnly WorkDate { get; set; }

    public string ProductionOrderNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int OrderQty { get; set; }
    public DateOnly DueDate { get; set; }
    public string WorkClassName { get; set; } = string.Empty;
    public int WorkClassId { get; set; }

    [Required(ErrorMessage = "Process is required.")]
    public int? ProcessId { get; set; }
    [Required(ErrorMessage = "Work pattern is required.")]
    public int? WorkPatternId { get; set; }

    public List<WorkReportWorkerInputViewModel> WorkReportWorkers { get; set; } = new();
    public List<SelectListItem> ProcessOptions { get; set; } = new();
    public List<SelectListItem> WorkPatternOptions { get; set; } = new();
}