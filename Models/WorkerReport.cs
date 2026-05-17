using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DailyWorkReport.Models;

[PrimaryKey(nameof(WorkReportId),nameof(WorkerId))]
public class WorkerReport
{
    public int WorkReportId { get; set; }
    public int WorkerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime {get; set;}
    public TimeSpan BreakTime {get;set;}
    public int Output { get; set; }

    public WorkReport WorkReport { get; set; } = null!;
    public Worker Worker { get; set; } = null!;


}
