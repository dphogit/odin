using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task<Temperature?> GetTemperatureByIdAsync(int id);

    public Task DeleteTemperatureAsync(Temperature temperature);
}
