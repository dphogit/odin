using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class TemperatureService(AppDbContext dbContext) : ITemperatureService
{
    public async Task<Temperature?> GetTemperatureByIdAsync(int id)
    {
        return await dbContext.Temperatures.FindAsync(id);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId)
    {
        return await dbContext.Temperatures.Where(t => t.DeviceId == deviceId).ToListAsync();
    }

    public async Task DeleteTemperatureAsync(Temperature temperature)
    {
        dbContext.Temperatures.Remove(temperature);
        await dbContext.SaveChangesAsync();
    }
}

public class TemperatureServiceException(string message) : Exception(message) { }
