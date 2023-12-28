namespace Odin.Shared.ApiDtos.Units;

public record ApiUnitDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record ApiCreateUnitDto
{
    public required string Name { get; init; }
    public required string Symbol { get; init; }
}

public record ApiUpdateUnitDto
{
    public string? Name { get; init; } = null;
    public string? Symbol { get; init; } = null;
}
