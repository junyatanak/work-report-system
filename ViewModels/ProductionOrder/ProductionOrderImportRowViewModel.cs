using CsvHelper.Configuration.Attributes;

namespace DailyWorkReport.ViewModels.ProductionOrder;

public class ProductionOrderImportRowViewModel
{
    [Name("OrderNumber")]
    public string? OrderNumber { get; set; }

    [Name("ProductCode")]
    public string? ProductCode { get; set; }

    [Name("OrderQty")]
    public string? OrderQty { get; set; }

    [Name("DueDate")]
    public string? DueDate { get; set; }
}