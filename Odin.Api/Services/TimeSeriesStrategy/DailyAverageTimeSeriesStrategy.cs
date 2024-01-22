using Odin.Api.Config;
using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services.TimeSeriesStrategy;

/// <summary>
///    Strategy for getting the daily average temperatures for a device for the last provided days.
/// </summary>
public class DailyAverageTimeSeriesStrategy(ITemperatureService temperatureService, int days) : TimeSeriesStrategyBase
{
    public override async Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataAsync(
        int deviceId,
        TimeSpan timezoneOffset)
    {
        var temperatures = await temperatureService.GetTemperaturesForDeviceAsync(deviceId, days);
        return GetDailyAverageDataPoints(temperatures, days, timezoneOffset);
    }

    public static IEnumerable<TimeSeriesDataPoint> GetDailyAverageDataPoints(
        IEnumerable<Temperature> temperatures,
        int days,
        TimeSpan timezoneOffset)
    {
        var format = DateTimeOffsetConstants.YearMonthDayFormat;

        var startDate = new DateTimeOffset(DateTimeOffset.Now.DateTime, timezoneOffset).Date.AddDays(-days);
        var dateRange = Enumerable.Range(0, days + 1).Select(i => startDate.AddDays(i)).ToList();

        return GetAverageDataPoints(new()
        {
            Temperatures = temperatures,
            DateRange = dateRange,
            TimezoneOffset = timezoneOffset,
            Format = format
        });
    }
}

/// <summary>
///     Strategy to get the daily average temperatures for a device for the last 30 days.
/// </summary>
public class MonthTimeSeriesStrategy(ITemperatureService temperatureService)
    : DailyAverageTimeSeriesStrategy(temperatureService, 30)
{ }

/// <summary>
///     Strategy to get the daily average temperatures for a device for the last 7 days.
/// </summary>
public class WeekTimeSeriesStrategy(ITemperatureService temperatureService)
    : DailyAverageTimeSeriesStrategy(temperatureService, 7)
{ }
