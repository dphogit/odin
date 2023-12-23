namespace Odin.Api.DTOs;

public record DeviceDTO
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; } = null;
    public string? Location { get; init; } = null;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record CreateDeviceDTO
{
    public required string Name { get; init; }
    public string? Description { get; init; } = null;
    public string? Location { get; init; } = null;
}

public record UpdateDeviceDTO
{
    public string? Name { get; init; } = null;
    public string? Description { get; init; } = null;
    public string? Location { get; init; } = null;
}
