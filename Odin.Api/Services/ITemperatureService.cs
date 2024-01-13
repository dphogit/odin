using Odin.Api.Config;
using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<IEnumerable<Temperature>> GetTemperaturesAsync();

    public Task<IEnumerable<Temperature>> GetTemperaturesAsync(bool withDevice);

    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(
        int deviceId,
        int days = TemperatureConfig.DefaultLastDays);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
