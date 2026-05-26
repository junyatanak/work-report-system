using System;

namespace DailyWorkReport.Models;

public class Worker
{
    public int Id { get; set; }
    public int WorkerNumber { get; set; }
    public string? Name { get; set; }
    public ICollection<WorkReportWorker> WorkReportWorkers { get; }= new List<WorkReportWorker>();

}
