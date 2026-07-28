namespace DailyWorkReport.ViewModels.ProductionOrder;

public class ProductionOrderDisplayViewModel
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int OrderQty { get; init; }
    public DateOnly DueDate { get; init; }
    public bool IsReported { get; init; }
}