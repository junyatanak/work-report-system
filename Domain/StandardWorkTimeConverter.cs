namespace DailyWorkReport.Domain;

public static class StandardWorkTimeConverter
{
    public static decimal ToStandardCycleSeconds(decimal pcsPerHour)
        => 3600m / pcsPerHour;

    public static decimal ToPcsPerHour(decimal standardCycleSeconds)
        => 3600m / standardCycleSeconds;
}