using System;

namespace DailyWorkReport.Models;

public class ProcessWorkPattern
{
    public int ProcessId { get; set; }
    public int WorkPatternId { get; set; }
    public Process Process { get; set; } = null!;
    public WorkPattern WorkPattern { get; set; } = null!;
}