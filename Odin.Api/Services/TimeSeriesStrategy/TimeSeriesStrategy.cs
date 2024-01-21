using Odin.Api.Config;
using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services.TimeSeriesStrategy;

public class DailyAverageTimeSeriesStrategy(ITemperatureService temperatureService, int days) : ITimeSeriesStrategy
{
    public async Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesData(int deviceId, TimeSpan timezoneOffset)
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

        var groupedByDate = dateRange.ToDictionary(
            date => date.ToString(format),
            _ => new List<double>()
        );

        foreach (var temperature in temperatures)
        {
            var date = temperature.Timestamp.ToOffset(timezoneOffset).Date.ToString(format);
            if (groupedByDate.TryGetValue(date, out var dateBucket))
            {
                dateBucket.Add(temperature.Value);
            }
        }

        return groupedByDate.Select(kvp =>
            new TimeSeriesDataPoint
            {
                Timestamp = kvp.Key,
                Value = kvp.Value.Count != 0 ? kvp.Value.Average() : null
            }
        );
    }
}

public class Last30DaysTimeSeriesStrategy(ITemperatureService temperatureService)
    : DailyAverageTimeSeriesStrategy(temperatureService, 30)
{ }

public class Last7DaysTimeSeriesStrategy(ITemperatureService temperatureService)
    : DailyAverageTimeSeriesStrategy(temperatureService, 7)
{ }

