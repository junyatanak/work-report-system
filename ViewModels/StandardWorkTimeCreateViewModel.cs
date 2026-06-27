using System.ComponentModel.DataAnnotations;
namespace DailyWorkReport.ViewModels;

public class StandardWorkTimeCreateViewModel
{
    [Required]
    [Display(Name = "WorkClass")]
    public int WorkClassId { get; set; }
    [Required]
    [Display(Name = "Process")]
    public int ProcessId { get; set; }
    [Required]
    [Display(Name = "WorkPattern")]
    public int WorkPatternId { get; set; }
    [Display(Name = "Standard Work Cycle (Seconds)")]
    public int? StandardCycleSeconds { get; set; }

}