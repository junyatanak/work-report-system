using System.ComponentModel.DataAnnotations;

namespace DailyWorkReport.ViewModels.ProductionOrder;

public class ProductionOrderCreateViewModel
{
    
    [Required(ErrorMessage = "Order number is required.")]  
    [Display(Name = "Order Number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product code is required.")]
    [Display(Name = "Product Code")]
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a valid product code.")]
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Order quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Order quantity must be at least 1.")]
    [Display(Name = "Order Quantity")]
    public int? OrderQty { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    [Display(Name = "Due Date")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}