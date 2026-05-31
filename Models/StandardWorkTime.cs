using System;
using Microsoft.EntityFrameworkCore;
namespace DailyWorkReport.Models;

[Index(nameof(ProcessId), nameof(WorkPatternId), nameof(WorkClassId), IsUnique = true)]
public class StandardWorkTime
{
    public int Id { get; set; }
    public int WorkClassId { get; set; }
    public int ProcessId { get; set; }
    public int WorkPatternId { get; set; }
    public int StandardCycleSeconds { get; set; }
    public WorkClass WorkClass { get; set; } = null!;
    public Process Process { get; set; } = null!;
    public WorkPattern WorkPattern { get; set; } = null!;

}