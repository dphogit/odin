using Odin.Api.DTOs;

namespace Odin.Api.Models;

public class Device : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; } = null;
    public string? Location { get; set; } = null;

    public static Device FromDTO(CreateDeviceDTO dto)
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
    public static DeviceDTO ToDTO(this Device device) => new()
    {
        Id = device.Id,
        Name = device.Name,
        Description = device.Description,
        Location = device.Location,
        CreatedAt = device.CreatedAt,
        UpdatedAt = device.UpdatedAt
    };
}
