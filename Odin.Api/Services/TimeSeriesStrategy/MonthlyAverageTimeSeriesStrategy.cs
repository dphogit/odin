using Odin.Api.Config;
using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services.TimeSeriesStrategy;

/// <summary>
///   Strategy for getting the monthly average temperatures for a device for the last provided months.
/// </summary>
public class MonthlyTimeSeriesStrategy(ITemperatureService temperatureService, int months) : TimeSeriesStrategyBase
{
    public override async Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataAsync(
        int deviceId,
        TimeSpan timezoneOffset)
    {
        var startDate = new DateTimeOffset(DateTimeOffset.Now.DateTime, timezoneOffset).Date.AddMonths(-months);
        var endDate = new DateTimeOffset(DateTimeOffset.Now.DateTime, timezoneOffset).Date;
        var days = (endDate - startDate).Days;

        var temperatures = await temperatureService.GetTemperaturesForDeviceAsync(deviceId, days);
        return GetMonthlyAverageDataPoints(temperatures, months, timezoneOffset);
    }

    public static IEnumerable<TimeSeriesDataPoint> GetMonthlyAverageDataPoints(
        IEnumerable<Temperature> temperatures,
        int months,
        TimeSpan timezoneOffset)
    {
        var format = DateTimeOffsetConstants.YearMonthFormat;

        var startDate = new DateTimeOffset(DateTimeOffset.Now.DateTime, timezoneOffset).Date.AddMonths(-months);
        var dateRange = Enumerable.Range(0, months + 1).Select(startDate.AddMonths);

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
///   Strategy to get the monthly average temperatures for a device for the last year (12 months).
/// </summary>
/// <param name="temperatureService"></param>
public class YearTimeSeriesStrategy(ITemperatureService temperatureService)
    : MonthlyTimeSeriesStrategy(temperatureService, 12)
{ }
