using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<int> CountTotalTemperaturesAsync();

    public Task<int> CountTotalTemperaturesForDeviceAsync(int deviceId);

    public Task<IEnumerable<Temperature>> GetTemperaturesAsync(GetTemperatureOptions options);

    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId, int days = 30);

    public Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataForDeviceAsync(
        int deviceId,
        TimeRange range,
        TimeSpan timezoneOffset);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
