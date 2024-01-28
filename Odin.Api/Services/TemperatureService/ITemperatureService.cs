using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<int> CountTotalTemperaturesAsync();

    /// <summary>
    ///     Counts total number of temperatures matching the given options. Useful for providing pagination metadata.
    /// </summary>
    public Task<int> CountTotalTemperaturesAsync(GetTemperatureOptions options);

    public Task<IEnumerable<Temperature>> GetTemperaturesAsync(GetTemperatureOptions options);

    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId, int days = 30);

    public Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataForDeviceAsync(
        int deviceId,
        TimeRange range,
        TimeSpan timezoneOffset);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
