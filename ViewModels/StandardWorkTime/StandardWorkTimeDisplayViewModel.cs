using System.ComponentModel.DataAnnotations;
namespace DailyWorkReport.ViewModels.StandardWorkTime;

public class StandardWorkTimeDisplayViewModel
{
    public int Id { get; init; }
    [Display(Name = "Work Class")]
    public string WorkClassName { get; init; } = string.Empty;
    [Display(Name = "Process")]
    public string ProcessName { get; init; } = string.Empty;
    [Display(Name = "Work Pattern")]
    public string WorkPatternName { get; init; } = string.Empty;
    [Display(Name = "Standard Work Time (pcs/hour)")]
    [DisplayFormat(NullDisplayText = "-")]
    public decimal? StandardWorkTime { get; init; }
}