using System;

namespace DailyWorkReport.Models;

public class Worker
{
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = String.Empty;

    public ICollection<WorkerReport> WorkerReports { get; set; } = new List<WorkerReport>();

}
