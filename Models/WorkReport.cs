using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyWorkReport.Models;

public class WorkReport
{
    public int WorkReportId { get; set; }
    public string OrderId {get; set;} = String.Empty;
    public DateOnly WorkDate { get; set; }
    public int ProcessId { get; set; }
    public int UserId { get; set; }

    public Process Process { get; set; } = null!;
    public ICollection<WorkerReport> WorkerReports { get; set; } = new List<WorkerReport>();

}
