using Odin.Shared.ApiDtos.Devices;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Models;

public class Temperature : Measurement
{
    public static Temperature FromDto(ApiAddTemperatureDto dto)
    {
        return new()
        {
            DeviceId = dto.DeviceId,
            Timestamp = dto.Timestamp,
            Value = dto.DegreesCelsius,
        };
    }
}

public static class TemperatureExtensions
{
    public static ApiTemperatureDto ToDto(this Temperature temperature, ApiDeviceDto? device = null)
    {
        return new()
        {
            Id = temperature.Id,
            DeviceId = temperature.DeviceId,
            Timestamp = temperature.Timestamp,
            DegreesCelsius = temperature.Value,
            Device = device,
        };
    }
}
