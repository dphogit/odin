namespace Odin.Api.Config;

public static class HttpRequestExtensions
{
    /// <summary>
    ///    Gets the timezone offset (in minutes) between the user's timezone and UTC from the request headers
    ///    ("X-Timezone-Offset") If the user's timezone is ahead of UTC, the offset will be positive. If the user's
    ///    timezone is behind UTC, the offset will be negative.
    /// </summary>
    public static TimeSpan GetTimezoneOffset(this HttpRequest httpRequest)
    {
        var timezoneOffset = TimeSpan.FromMinutes(0);
        if (httpRequest.Headers.TryGetValue("X-Timezone-Offset", out var values))
        {
            if (int.TryParse(values, out var offsetMinutes))
            {
                timezoneOffset = TimeSpan.FromMinutes(offsetMinutes);
            }
        }
        return timezoneOffset;
    }
}
