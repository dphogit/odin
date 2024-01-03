using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IDeviceService
{
    public Task<IEnumerable<Device>> GetDevicesAsync();

    /// <summary>
    /// Gets a device by id with no measurements included.
    /// </summary>
    public Task<Device?> GetDeviceByIdAsync(int id);

    /// <summary>
    /// Gets a device by id and includes measurements of type <typeparamref name="TMeasurement"/>. Each measurement
    /// also includes it's associated unit.
    /// </summary>
    public Task<Device?> GetDeviceByIdAsync<TMeasurement>(int id) where TMeasurement : Measurement;

    public Task<Device?> GetDeviceByNameAsync(string name);

    public Task CreateDeviceAsync(Device device);

    public Task AddMeasurementForDeviceAsync(int deviceId, Measurement measurement);

    public Task AddMeasurementForDeviceAsync(Device device, Measurement measurement);

    public Task UpdateDeviceAsync(Device device);

    public Task DeleteDeviceAsync(Device device);
}
