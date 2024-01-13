using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Endpoints;

public static class TemperatureEndpoints
{
    public static RouteGroupBuilder MapTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", GetAllTemperatures).WithName(nameof(GetAllTemperatures));
        builder.MapGet("/{temperatureId}", GetTemperature).WithName(nameof(GetTemperature));
        builder.MapDelete("/{temperatureId}", DeleteTemperature).WithName(nameof(DeleteTemperature));
        return builder;
    }

    public static async Task<Ok<IEnumerable<ApiTemperatureDto>>> GetAllTemperatures(
        ITemperatureService temperatureService,
        bool withDevice = false
    )
    {
        var temperatures = await temperatureService.GetTemperaturesAsync(withDevice);
        var temperatureDtos = temperatures.Select(t => t.ToDto(t.Device?.ToDto()));
        return TypedResults.Ok(temperatureDtos);
    }

    public static async Task<Results<Ok<ApiTemperatureDto>, NotFound>> GetTemperature(
        ITemperatureService temperatureService,
        int temperatureId)
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(temperature.ToDto());
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTemperature(
        ITemperatureService temperatureService,
        int temperatureId
    )
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null)
            return TypedResults.NotFound();

        await temperatureService.DeleteTemperatureAsync(temperature);
        return TypedResults.NoContent();
    }
}
