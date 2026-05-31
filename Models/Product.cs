using System;
using Microsoft.EntityFrameworkCore;
namespace DailyWorkReport.Models;

[Index(nameof(ProductCode), IsUnique = true)]
public class Product
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int WorkClassId { get; set; }
    public ICollection<ProductionOrder> ProductionOrders { get;} = new List<ProductionOrder>();
    public WorkClass WorkClass { get; set; } = null!;
}