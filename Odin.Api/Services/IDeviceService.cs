using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IDeviceService
{
    public Task<IEnumerable<Device>> GetDevicesAsync();

    public Task<Device?> GetDeviceByIdAsync(int id);

    public Task<Device?> GetDeviceByNameAsync(string name);

    public Task CreateDeviceAsync(Device device);

    public Task AddMeasurementForDeviceAsync(int deviceId, Measurement measurement);

    public Task AddMeasurementForDeviceAsync(Device device, Measurement measurement);

    public Task UpdateDeviceAsync(Device device);

    public Task DeleteDeviceAsync(Device device);
}
