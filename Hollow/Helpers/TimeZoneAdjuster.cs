using System;

namespace Hollow.Helpers;

public class TimeZoneAdjuster(int knownTimeZoneOffsetHours)
{
    private readonly TimeZoneInfo _knownTimeZone = TimeZoneInfo.CreateCustomTimeZone("ServerTimeZone", TimeSpan.FromHours(knownTimeZoneOffsetHours), "ServerTimeZone", "ServerTimeZone");
    public static readonly TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

    // public DateTime ConvertToKnownTimeZone(DateTime dateTime)
    // {
    //     return TimeZoneInfo.ConvertTime(dateTime, _localTimeZone, _knownTimeZone);
    // }

    public string ConvertToLocalTimeZone(string knownDateTime)
    {
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(knownDateTime), _knownTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, LocalTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
    }
}

public static class DateTimeDisplayExtensions
{
    public static string ToUtcPrefixTimeZone(this int offsetHours)
    {
        return offsetHours >= 0 ? $"UTC+{offsetHours}" : $"UTC{offsetHours}";
    }
}