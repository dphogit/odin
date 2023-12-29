using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Endpoints;

public static class TemperatureEndpoints
{
    public static RouteGroupBuilder MapTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/{id}", GetTemperatureById).WithName(nameof(GetTemperatureById));
        builder.MapPost("/", AddTemperature).WithName(nameof(AddTemperature));
        return builder;
    }

    public static async Task<Results<Ok<ApiTemperatureDto>, NotFound>> GetTemperatureById(
        ITemperatureService temperatureService,
        int id)
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(id);

        if (temperature is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(temperature.ToDto());
    }

    public static async Task<CreatedAtRoute<ApiTemperatureDto>> AddTemperature(
        ITemperatureService temperatureService,
        ApiAddTemperatureDto addTemperatureDto)
    {
        var temperature = Temperature.FromDto(addTemperatureDto);
        await temperatureService.AddTemperatureAsync(temperature);

        var temperatureDTO = temperature.ToDto();

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetTemperatureById),
            routeValues: new { id = temperatureDTO.Id.ToString() },
            value: temperatureDTO
        );
    }
}
