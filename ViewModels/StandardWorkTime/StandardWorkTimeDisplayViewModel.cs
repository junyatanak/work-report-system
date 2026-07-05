using System.ComponentModel.DataAnnotations;
namespace DailyWorkReport.ViewModels.StandardWorkTime;

public class StandardWorkTimeDisplayViewModel
{
    public int Id { get; init; }
    [Display(Name = "WorkClass")]
    public string WorkClassName { get; init; } = string.Empty;
    [Display(Name = "Process")]
    public string ProcessName { get; init; } = string.Empty;
    [Display(Name = "WorkPattern")]
    public string WorkPatternName { get; init; } = string.Empty;
    [Display(Name = "Standard Work Time (pcs/hour)")]
    public decimal? StandardWorkTime { get; init; }
}