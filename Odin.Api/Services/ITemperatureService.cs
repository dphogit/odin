using Odin.Api.Config;
using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<int> CountTotalTemperaturesAsync();

    public Task<IEnumerable<Temperature>> GetTemperaturesAsync(
        bool withDevice = false,
        int page = 1,
        int limit = TemperatureConfig.DefaultPaginationLimit);

    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(
        int deviceId,
        int days = TemperatureConfig.DefaultLastDays);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
