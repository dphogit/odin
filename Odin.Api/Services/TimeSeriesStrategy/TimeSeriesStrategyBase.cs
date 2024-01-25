using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services.TimeSeriesStrategy;

public abstract class TimeSeriesStrategyBase : ITimeSeriesStrategy
{
    public record GetAverageDataPointsConfig
    {
        public required IEnumerable<Temperature> Temperatures { get; init; }
        public required IEnumerable<DateTime> DateRange { get; init; }
        public required TimeSpan TimezoneOffset { get; init; }
        public required string Format { get; init; }
    }

    public abstract Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataAsync(int deviceId, TimeSpan timezoneOffset);

    protected static IEnumerable<TimeSeriesDataPoint> GetAverageDataPoints(GetAverageDataPointsConfig config)
    {
        var buckets = config.DateRange.ToDictionary(
            date => date.ToString(config.Format),
            _ => new List<double>()
        );

        foreach (var temperature in config.Temperatures)
        {
            var date = temperature.Timestamp.ToOffset(config.TimezoneOffset).ToString(config.Format);
            if (buckets.TryGetValue(date, out var bucket))
            {
                bucket.Add(temperature.Value);
            }
        }

        return buckets.Select(kvp =>
            new TimeSeriesDataPoint
            {
                Timestamp = kvp.Key,
                Value = kvp.Value.Count != 0 ? kvp.Value.Average() : null
            }
        );
    }
}
