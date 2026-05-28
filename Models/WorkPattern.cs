using System;

namespace DailyWorkReport.Models;

public class WorkPattern
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public ICollection<WorkReport> WorkReports {get;} = new List<WorkReport>();
    public ICollection<ProcessWorkPattern> ProcessWorkPatterns {get;} = new List<ProcessWorkPattern>();
    public ICollection<StandardWorkTime> StandardWorkTimes { get;} = new List<StandardWorkTime>();

}
