using System;

namespace DailyWorkReport.Models;

public class Process
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = String.Empty;
    public string Operation { get; set; } = String.Empty;
    public int OutputPerCycle { get; set; }
    public double TargetCycleTime { get; set; }

    public ICollection<WorkReport> WorkReports {get;} = new List<WorkReport>();

}
