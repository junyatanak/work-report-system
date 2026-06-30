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
    [Display(Name = "Standard Work Time (pcs/hour)")]
    [Range(typeof(decimal), "1", "100000")]
    public decimal? StandardWorkTime { get; set; }
}