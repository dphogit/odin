using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Units;

namespace Odin.Api.Endpoints;

public static class UnitEndpoints
{
    public static RouteGroupBuilder MapUnitEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", AddUnit).WithName(nameof(AddUnit));
        builder.MapGet("/", GetUnits).WithName(nameof(GetUnits));
        builder.MapGet("/{id}", GetUnitById).WithName(nameof(GetUnitById));
        builder.MapPut("/{id}", UpdateUnit).WithName(nameof(UpdateUnit));
        builder.MapDelete("/{id}", DeleteUnit).WithName(nameof(DeleteUnit));
        return builder;
    }

    public static async Task<Ok<List<ApiUnitDto>>> GetUnits(IUnitService unitService)
    {
        var units = await unitService.GetUnitsAsync();
        var unitDTOs = units.Select(unit => unit.ToDto()).ToList();
        return TypedResults.Ok(unitDTOs);
    }

    public static async Task<Results<Ok<ApiUnitDto>, NotFound>> GetUnitById(IUnitService unitService, int id)
    {
        var unit = await unitService.GetUnitByIdAsync(id);

        if (unit is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(unit.ToDto());
    }

    public async static Task<CreatedAtRoute<ApiUnitDto>> AddUnit(
        IUnitService unitService,
        ApiCreateUnitDto createUnitDto)
    {
        var unit = Unit.FromDto(createUnitDto);
        await unitService.CreateUnitAsync(unit);

        var unitDTO = unit.ToDto();

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetUnitById),
            routeValues: new { id = unitDTO.Id.ToString() },
            value: unitDTO
        );
    }

    public static async Task<Results<NotFound, NoContent>> UpdateUnit(
        IUnitService unitService,
        int id,
        ApiUpdateUnitDto updateUnitDto)
    {
        var unit = await unitService.GetUnitByIdAsync(id);

        if (unit is null)
            return TypedResults.NotFound();

        unit.Name = updateUnitDto.Name ?? unit.Name;
        unit.Symbol = updateUnitDto.Symbol ?? unit.Symbol;

        await unitService.UpdateUnitAsync(unit);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteUnit(IUnitService unitService, int id)
    {
        var unit = await unitService.GetUnitByIdAsync(id);

        if (unit is null)
            return TypedResults.NotFound();

        await unitService.DeleteUnitAsync(unit);

        return TypedResults.NoContent();
    }
}
