using System.Text.Json.Serialization;
using Odin.Shared.ApiDtos.Devices;

namespace Odin.Shared.ApiDtos.Temperatures;

public record ApiTemperatureDto
{
    public required int Id { get; init; }

    public required int DeviceId { get; init; }

    public required double DegreesCelsius { get; init; }

    public required DateTimeOffset Timestamp { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ApiDeviceDto? Device { get; init; }
}

public record ApiAddTemperatureDto
{
    public required int DeviceId { get; init; }

    public required double DegreesCelsius { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
