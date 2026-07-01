namespace DailyWorkReport.Domain;

public static class StandardWorkTimeConverter
{
    public static decimal? ToStandardCycleSeconds(decimal? pcsPerHour)
        => pcsPerHour.HasValue 
            ? 3600m / pcsPerHour.Value 
            : null;

    public static decimal? ToPcsPerHour(decimal? standardCycleSeconds)
        => standardCycleSeconds.HasValue 
            ? 3600m / standardCycleSeconds.Value 
            : null;
}