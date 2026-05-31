using System;
using Microsoft.EntityFrameworkCore;
namespace DailyWorkReport.Models;

[Index(nameof(Name), IsUnique = true)]
public class WorkClass
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;

    public ICollection<Product> Products { get;} = new List<Product>();
    public ICollection<StandardWorkTime> StandardWorkTimes { get;} = new List<StandardWorkTime>();

    
} 