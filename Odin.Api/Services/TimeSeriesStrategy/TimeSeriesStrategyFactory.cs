using Odin.Api.Endpoints.ResponseSchemas;

namespace Odin.Api.Services.TimeSeriesStrategy;

public class TimeSeriesStrategyFactory(ITemperatureService temperatureService)
{
    public ITimeSeriesStrategy CreateTimeSeriesStrategy(TimeRange range)
    {
        return range switch
        {
            TimeRange.Year => new YearTimeSeriesStrategy(temperatureService),
            TimeRange.Month => new MonthTimeSeriesStrategy(temperatureService),
            TimeRange.Days => new WeekTimeSeriesStrategy(temperatureService),
            _ => throw new ArgumentException($"Time range \"{range}\" is not supported")
        };
    }
}
