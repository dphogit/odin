using System.Text.Json.Serialization;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Shared.ApiDtos.Devices;

public record ApiDeviceDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Location { get; init; } = null;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<ApiTemperatureDto>? Temperatures { get; init; } = null;
}

public record ApiCreateDeviceDto
{
    public required string Name { get; init; }
    public string? Description { get; init; } = null;
    public string? Location { get; init; } = null;
}

public record ApiUpdateDeviceDto
{
    public string? Name { get; init; } = null;
    public string? Description { get; init; } = null;
    public string? Location { get; init; } = null;
}
