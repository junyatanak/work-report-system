namespace DailyWorkReport.ViewModels.Product;

public class ProductDisplayViewModel
{    
    public int Id { get; init; }
    public string ProductCode { get; init; } = String.Empty;
    public string Name { get; init; } = String.Empty;
    public string WorkClassName { get; init; } = String.Empty;
}