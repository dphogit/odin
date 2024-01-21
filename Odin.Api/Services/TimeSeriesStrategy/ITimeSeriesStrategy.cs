using Odin.Api.Endpoints.ResponseSchemas;

namespace Odin.Api.Services.TimeSeriesStrategy;

public interface ITimeSeriesStrategy
{
    public Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesData(int deviceId, TimeSpan timezoneOffset);
}
