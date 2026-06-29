namespace DailyWorkReport.Domain;

public static class StandardWorkTimeConverter
{
    public static int ToStandardCycleSeconds(decimal pcsPerHour)
        => (int)Math.Round(3600m / pcsPerHour);

    public static decimal ToPcsPerHour(int standardCycleSeconds)
        => 3600m / standardCycleSeconds;
}