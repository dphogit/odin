namespace Odin.Shared.ApiDtos.Temperatures;

public record ApiTemperatureDto
{
    public required int Id { get; init; }
    public required int DeviceId { get; init; }
    public required double DegreesCelsius { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}

public record ApiAddTemperatureDto
{
    public required int DeviceId { get; init; }
    public required double DegreesCelsius { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
