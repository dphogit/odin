using Odin.Api.Endpoints.ResponseSchemas;

namespace Odin.Api.Services.TimeSeriesStrategy;

public interface ITimeSeriesStrategy
{
    public Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataAsync(int deviceId, TimeSpan timezoneOffset);
}
