using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
