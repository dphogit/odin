using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
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

    public async Task<int> CountTotalTemperaturesAsync(GetTemperatureOptions options)
    {
        var query = BuildFilteredGetTemperaturesQuery(options);
        return await query.CountAsync();
    }

    public async Task<IEnumerable<Temperature>> GetTemperaturesAsync(GetTemperatureOptions options)
    {
        var withDevice = options.WithDevice;
        var page = options.Page;
        var limit = options.Limit;
        var timestampSort = options.TimestampSort;

        var query = BuildFilteredGetTemperaturesQuery(options);

        if (withDevice is true)
            query = query.Include(t => t.Device);

        if (timestampSort == TimestampSortOptions.Ascending)
            query = query.OrderBy(t => t.Timestamp);
        else
            query = query.OrderByDescending(t => t.Timestamp);

        return await query
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

    private IQueryable<Temperature> BuildFilteredGetTemperaturesQuery(GetTemperatureOptions options)
    {
        var minValue = options.MinValue;
        var maxValue = options.MaxValue;

        var query = dbContext.Temperatures.AsQueryable();

        if (minValue is not null)
            query = query.Where(t => t.Value >= minValue);

        if (maxValue is not null)
            query = query.Where(t => t.Value <= maxValue);

        return query;
    }
}
