using Microsoft.EntityFrameworkCore;
using Odin.Shared.ApiDtos.Devices;

namespace Odin.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Device : CreatedAtAndUpdatedAtEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; } = null;

    public string? Location { get; set; } = null;

    public ICollection<Measurement> Measurements { get; } = new List<Measurement>();

    public static Device FromDto(ApiCreateDeviceDto dto)
    {
        return new()
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location
        };
    }
}

public static class DeviceExtensions
{
    /// <summary>
    /// Converts a <see cref="Device"/> to a <see cref="ApiDeviceDto"/>, with the option to include the
    /// <see cref="ApiDeviceDto.Temperatures"/> field when the <paramref name="withTemperaturesField"/>
    /// is set to true (default: false).
    /// </summary>
    public static ApiDeviceDto ToDto(this Device device, bool withTemperaturesField = false)
    {
        var temperatures = withTemperaturesField
            ? device.Measurements.OfType<Temperature>().Select(temperature => temperature.ToDto()).ToList()
            : null;

        var dto = new ApiDeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Description = device.Description,
            Location = device.Location,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
            Temperatures = temperatures
        };

        return dto;
    }
}
