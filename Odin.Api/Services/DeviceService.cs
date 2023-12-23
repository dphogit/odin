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

    public async Task CreateDeviceAsync(Device device)
    {
        dbContext.Devices.Add(device);
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
