using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IDeviceService
{
    public Task<IEnumerable<Device>> GetDevicesAsync();
    public Task<Device?> GetDeviceByIdAsync(int id);
    public Task CreateDeviceAsync(Device device);
    public Task UpdateDeviceAsync(Device device);
    public Task DeleteDeviceAsync(Device device);
}
