using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyWorkReport.Models;

public class WorkReport
{
    public int Id { get; set; }
    public DateOnly WorkDate { get; set; }
    public int ProductionOrderId {get; set;}
    public int ProcessId { get; set; }
    public int WorkPatternId { get; set; }
    public int UserId { get; set; }

    public ProductionOrder ProductionOrder { get; set; } = null!;
    public Process Process { get; set; } = null!;
    public WorkPattern WorkPattern { get; set; } = null!;
    public ICollection<WorkReportWorker> WorkReportWorkers { get;} = new List<WorkReportWorker>();
    public ApplicationUser User { get; set; } = null!;

}
