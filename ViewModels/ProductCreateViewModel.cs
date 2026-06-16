using System.ComponentModel.DataAnnotations;
namespace DailyWorkReport.ViewModels;

public class ProductCreateViewModel
{
    [Required]
    public string ProductCode { get; set; } = String.Empty;
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public int? WorkClassId { get; set; }
}
