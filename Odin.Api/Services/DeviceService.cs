using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class DeviceService(AppDbContext dbContext, IUnitService unitService) : IDeviceService
{
    public async Task<IEnumerable<Device>> GetDevicesAsync()
    {
        return await dbContext.Devices.ToListAsync();
    }

    public async Task<Device?> GetDeviceByIdAsync(int id)
    {
        return await dbContext.Devices.FindAsync(id);
    }

    public async Task<Device?> GetDeviceByNameAsync(string name)
    {
        return await dbContext.Devices.SingleOrDefaultAsync(device => device.Name == name);
    }

    public async Task CreateDeviceAsync(Device device)
    {
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddMeasurementForDeviceAsync(int deviceId, Measurement measurement)
    {
        var device = await GetDeviceByIdAsync(deviceId) ??
            throw new ArgumentException($"Device with id {deviceId} does not exist");

        await AddMeasurementForDeviceAsync(device, measurement);
    }

    public async Task AddMeasurementForDeviceAsync(Device device, Measurement measurement)
    {
        // Consider using a factory once the pattern for creating units can be precisely defined
        // (maybe even extract to a new IDeviceMeasurementService?)
        Unit unit = measurement switch
        {
            Temperature _ => await unitService.GetUnitByNameAsync("Degrees Celsius") ??
                                throw new ArgumentException("Degrees Celsius unit not found"),
            _ => throw new ArgumentException($"Measurement type {measurement.GetType()} is not supported"),
        };

        measurement.Unit = unit;
        device.Measurements.Add(measurement);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateDeviceAsync(Device device)
    {
        dbContext.Devices.Update(device);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteDeviceAsync(Device device)
    {
        dbContext.Devices.Remove(device);
        await dbContext.SaveChangesAsync();
    }
}
