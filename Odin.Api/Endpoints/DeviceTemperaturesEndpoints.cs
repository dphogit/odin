using Microsoft.AspNetCore.Http.HttpResults;
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
        builder.MapGet("/{temperatureId}", GetTemperatureForDevice).WithName(nameof(GetTemperatureForDevice));
        builder.MapPost("/", AddTemperatureForDevice).WithName(nameof(AddTemperatureForDevice));
        builder.MapDelete("/{temperatureId}", DeleteTemperatureForDevice).WithName(nameof(DeleteTemperatureForDevice));
        return builder;
    }

    public static async Task<Results<Ok<IEnumerable<ApiTemperatureDto>>, NotFound>> GetAllDeviceTemperatures(
        ITemperatureService temperatureService,
        IDeviceService deviceService,
        int deviceId
    )
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperatures = await temperatureService.GetTemperaturesForDeviceAsync(deviceId);
        var temperatureDtos = temperatures.Select(t => t.ToDto());
        return TypedResults.Ok(temperatureDtos);
    }

    public static async Task<Results<Ok<ApiTemperatureDto>, NotFound>> GetTemperatureForDevice(
        IDeviceService deviceService,
        ITemperatureService temperatureService,
        int deviceId,
        int temperatureId)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null || temperature.DeviceId != deviceId)
            return TypedResults.NotFound();

        return TypedResults.Ok(temperature.ToDto());
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
            routeName: nameof(GetTemperatureForDevice),
            routeValues: new { deviceId, temperatureId = temperatureDTO.Id.ToString() },
            value: temperatureDTO
        );
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTemperatureForDevice(
        IDeviceService deviceService,
        ITemperatureService temperatureService,
        int temperatureId,
        int deviceId)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null || temperature.DeviceId != deviceId)
            return TypedResults.NotFound();

        await temperatureService.DeleteTemperatureAsync(temperature);

        return TypedResults.NoContent();
    }
}
