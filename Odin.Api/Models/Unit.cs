using Microsoft.EntityFrameworkCore;
using Odin.Shared.ApiDtos.Units;

namespace Odin.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Unit : CreatedAtAndUpdatedAtEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;

    public static Unit FromDto(ApiCreateUnitDto createUnitDto)
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
    public static ApiUnitDto ToDto(this Unit unit) => new()
    {
        Id = unit.Id,
        Name = unit.Name,
        Symbol = unit.Symbol,
        CreatedAt = unit.CreatedAt,
        UpdatedAt = unit.UpdatedAt
    };
}
