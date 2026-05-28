using System;
namespace DailyWorkReport.Models;

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