using Odin.Shared.ApiDtos.Units;

namespace Odin.Api.Models;

public class Unit : CreatedAtAndUpdatedAtEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;

    public static Unit FromDTO(ApiCreateUnitDto createUnitDto)
    {
        return new()
        {
            Name = createUnitDto.Name,
            Symbol = createUnitDto.Symbol
        };
    }
}

public static class UnitExtensions
{
    public static ApiUnitDto ToDTO(this Unit unit) => new()
    {
        Id = unit.Id,
        Name = unit.Name,
        Symbol = unit.Symbol,
        CreatedAt = unit.CreatedAt,
        UpdatedAt = unit.UpdatedAt
    };
}
