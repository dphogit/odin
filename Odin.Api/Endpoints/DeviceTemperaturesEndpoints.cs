using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Config;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Endpoints;

/// <summary>
/// Temperatures endpoints attached onto a targetted device's endpoint group i.e. /devices/{deviceId}/temperatures.
/// </summary>
public static class DeviceTemperaturesEndpoints
{
    public static RouteGroupBuilder MapDeviceTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", GetAllDeviceTemperatures).WithName(nameof(GetAllDeviceTemperatures));
        builder.MapPost("/", AddTemperatureForDevice).WithName(nameof(AddTemperatureForDevice));
        return builder;
    }

    public static async Task<Results<Ok<IEnumerable<ApiTemperatureDto>>, NotFound>> GetAllDeviceTemperatures(
        ITemperatureService temperatureService,
        IDeviceService deviceService,
        int deviceId,
        int days = TemperatureConfig.DefaultLastDays
    )
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperatures = await temperatureService.GetTemperaturesForDeviceAsync(deviceId, days);
        var temperatureDtos = temperatures.Select(t => t.ToDto());
        return TypedResults.Ok(temperatureDtos);
    }

    public static async Task<Results<CreatedAtRoute<ApiTemperatureDto>, NotFound>> AddTemperatureForDevice(
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
            routeName: nameof(TemperatureEndpoints.GetTemperature),
            routeValues: new { temperatureId = temperatureDTO.Id.ToString() },
            value: temperatureDTO
        );
    }
}
