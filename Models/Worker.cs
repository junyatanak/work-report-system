using System;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Models;

[Index(nameof(WorkerNumber), IsUnique = true)]
public class Worker
{
    public int Id { get; set; }
    public int WorkerNumber { get; set; }
    public string? Name { get; set; }
    public ICollection<WorkReportWorker> WorkReportWorkers { get; }= new List<WorkReportWorker>();

}
