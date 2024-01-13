using Microsoft.EntityFrameworkCore;
using Odin.Api.Config;
using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class TemperatureService(AppDbContext dbContext) : ITemperatureService
{
    public async Task<IEnumerable<Temperature>> GetTemperaturesAsync()
    {
        return await GetTemperaturesAsync(false);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesAsync(bool withDevice)
    {
        if (withDevice)
            return await dbContext.Temperatures
                .Include(t => t.Device)
                .ToListAsync();

        return await dbContext.Temperatures.ToListAsync();
    }

    public async Task<Temperature?> GetTemperatureByIdAsync(int id)
    {
        return await dbContext.Temperatures.FindAsync(id);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId)
    {
        return await GetTemperaturesForDeviceAsync(deviceId, TemperatureConfig.DefaultLastDays);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(
        int deviceId,
        int days = TemperatureConfig.DefaultLastDays)
    {
        // Get last X days including today's values => benchmark previous days against today starting at midnight.
        var today = DateTimeOffset.UtcNow.Date;
        var start = today.AddDays(-days);
        return await dbContext.Temperatures
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= start)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task DeleteTemperatureAsync(Temperature temperature)
    {
        dbContext.Temperatures.Remove(temperature);
        await dbContext.SaveChangesAsync();
    }
}

public class TemperatureServiceException(string message) : Exception(message) { }
