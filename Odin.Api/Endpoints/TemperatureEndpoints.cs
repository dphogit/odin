using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Endpoints;

public static class TemperatureEndpoints
{
    public static RouteGroupBuilder MapTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/{temperatureId}", GetTemperatureById).WithName(nameof(GetTemperatureById));
        builder.MapPost("/", AddTemperatureMeasurement).WithName(nameof(AddTemperatureMeasurement));
        return builder;
    }

    public static async Task<Results<Ok<ApiTemperatureDto>, NotFound>> GetTemperatureById(
        ITemperatureService temperatureService,
        int temperatureId)
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(temperature.ToDto());
    }

    public static async Task<Results<CreatedAtRoute<ApiTemperatureDto>, NotFound>> AddTemperatureMeasurement(
        IDeviceService deviceService,
        ApiAddTemperatureDto addTemperatureDto,
        int deviceId)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperature = Temperature.FromDto(addTemperatureDto);

        await deviceService.AddMeasurementForDeviceAsync(device, temperature);

        var temperatureDTO = temperature.ToDto();

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetTemperatureById),
            routeValues: new { deviceId, temperatureId = temperatureDTO.Id.ToString() },
            value: temperatureDTO
        );
    }
}
