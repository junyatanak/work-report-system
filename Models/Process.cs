using System;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Models;

[Index(nameof(Name), IsUnique = true)]
public class Process
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public ICollection<WorkReport> WorkReports {get;} = new List<WorkReport>();
    public ICollection<StandardWorkTime> StandardWorkTimes { get;} = new List<StandardWorkTime>(); 

}
