using Odin.Api.Endpoints.ResponseSchemas;

namespace Odin.Api.Services.TimeSeriesStrategy;

public class TimeSeriesStrategyFactory(ITemperatureService temperatureService)
{
    public ITimeSeriesStrategy CreateTimeSeriesStrategy(TimeRange range)
    {
        return range switch
        {
            TimeRange.Last30Days => new Last30DaysTimeSeriesStrategy(temperatureService),
            TimeRange.Last7Days => new Last7DaysTimeSeriesStrategy(temperatureService),
            _ => throw new ArgumentException($"Time range \"{range}\" is not supported")
        };
    }
}
