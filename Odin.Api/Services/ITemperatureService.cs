using Odin.Api.Models;

namespace Odin.Api.Services;

public interface ITemperatureService
{
    public Task AddTemperatureAsync(Temperature temperature);
    public Task<Temperature?> GetTemperatureByIdAsync(int id);
}
