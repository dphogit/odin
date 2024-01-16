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

    public ICollection<Measurement> Measurements { get; } = [];

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
    public static ApiDeviceDto ToDto(this Device device)
    {
        return new ApiDeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Description = device.Description,
            Location = device.Location,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
        };
    }
}
