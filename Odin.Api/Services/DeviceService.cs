using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class DeviceService(AppDbContext dbContext) : IDeviceService
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
        measurement.Unit = Units.GetAssociatedUnit(measurement);
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
