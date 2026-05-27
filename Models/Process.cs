using System;

namespace DailyWorkReport.Models;

public class Process
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public ICollection<WorkReport> WorkReports {get;} = new List<WorkReport>();

}
