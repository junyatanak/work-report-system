using System;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Models;

[Index(nameof(WorkReportId), nameof(WorkerId), IsUnique = true)]
public class WorkReportWorker
{
    public int Id { get; set; }
    public int WorkReportId { get; set; }
    public int WorkerId { get; set; }

    [Precision(0)]
    public DateTime StartAt { get; set; }
    [Precision(0)]
    public DateTime EndAt { get; set; }
    public int BreakMinutes { get; set; } = 0;
    public int ProducedQty { get; set; }
    public WorkReport WorkReport { get; set; } = null!;
    public Worker Worker { get; set; } = null!;

}
