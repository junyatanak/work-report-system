using System.ComponentModel.DataAnnotations;

namespace DailyWorkReport.ViewModels.ProductionOrder;

public class ProductionOrderEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Order number is required.")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product code is required.")]
    public string ProductCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a valid product code.")]
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Order quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Order quantity must be at least 1.")]
    public int? OrderQty { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    public DateOnly DueDate { get; set; }
    
}