using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace DailyWorkReport.Models;

[Index(nameof(OrderNumber), IsUnique = true)]
public class ProductionOrder: IHasRowVersion
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = String.Empty;
    public int ProductId { get; set; }
    public int OrderQty { get; set; }
    public DateOnly DueDate{get; set; }
    public ICollection<WorkReport> WorkReports { get;} = new List<WorkReport>();
    public Product Product { get; set; } = null!;
    [ConcurrencyCheck]
    public Guid RowVersion { get; set; } = Guid.NewGuid();
}