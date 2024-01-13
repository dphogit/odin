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
}
