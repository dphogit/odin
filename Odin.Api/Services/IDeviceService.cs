using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IDeviceService
{
    Task<IEnumerable<Device>> GetDevicesAsync();
    Task<Device?> GetDeviceByIdAsync(int id);
    Task CreateDeviceAsync(Device device);
    Task UpdateDeviceAsync(Device device);
    Task DeleteDeviceAsync(Device device);
}
