using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Endpoints.Pagination;
using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.Models;
using Odin.Api.Services.TimeSeriesStrategy;

namespace Odin.Api.Services;

public class TemperatureService(AppDbContext dbContext) : ITemperatureService
{
    public async Task<int> CountTotalTemperaturesAsync()
    {
        return await dbContext.Temperatures.CountAsync();
    }

    public async Task<int> CountTotalTemperaturesForDeviceAsync(int deviceId)
    {
        return await dbContext.Temperatures.CountAsync(t => t.DeviceId == deviceId);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesAsync(
        bool withDevice = false,
        int page = 1,
        int limit = PaginationConstants.DefaultPaginationLimit)
    {
        var query = dbContext.Temperatures.AsQueryable();

        if (withDevice is true)
        {
            query = query.Include(t => t.Device);
        }

        return await query
            .OrderByDescending(t => t.Timestamp)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Temperature?> GetTemperatureByIdAsync(int id)
    {
        return await dbContext.Temperatures.FindAsync(id);
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesForDeviceAsync(int deviceId, int days = 30)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var start = today.AddDays(-days);
        return await dbContext.Temperatures
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= start)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeSeriesDataPoint>> GetTimeSeriesDataForDeviceAsync(
        int deviceId,
        TimeRange range,
        TimeSpan timezoneOffset)
    {
        var factory = new TimeSeriesStrategyFactory(this);
        var strategy = factory.CreateTimeSeriesStrategy(range);
        return await strategy.GetTimeSeriesDataAsync(deviceId, timezoneOffset);
    }

    public async Task DeleteTemperatureAsync(Temperature temperature)
    {
        dbContext.Temperatures.Remove(temperature);
        await dbContext.SaveChangesAsync();
    }
}

public class TemperatureServiceException(string message) : Exception(message) { }
