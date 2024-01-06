using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.Models;
using Odin.Api.Services;

namespace Odin.Api.Endpoints;

public static class DeviceEndpoints
{
    public static RouteGroupBuilder MapDeviceEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", GetDevices).WithName(nameof(GetDevices));
        builder.MapGet("/{deviceId}", GetDeviceById).WithName(nameof(GetDeviceById));
        builder.MapGet("/name/{name}", GetDeviceByName).WithName(nameof(GetDeviceByName));
        builder.MapPost("/", AddDevice).WithName(nameof(AddDevice));
        builder.MapPatch("/{deviceId}", UpdateDevice).WithName(nameof(UpdateDevice));
        builder.MapDelete("/{deviceId}", DeleteDevice).WithName(nameof(DeleteDevice));

        builder.MapGroup("/{deviceId}/temperatures").MapDeviceTemperatureEndpoints();

        return builder;
    }

    public static async Task<Ok<List<ApiDeviceDto>>> GetDevices(IDeviceService deviceService)
    {
        var devices = await deviceService.GetDevicesAsync();
        var deviceDTOs = devices.Select(device => device.ToDto()).ToList();
        return TypedResults.Ok(deviceDTOs);
    }

    public static async Task<Results<Ok<ApiDeviceDto>, NotFound>> GetDeviceById(
        IDeviceService deviceService,
        int deviceId)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(device.ToDto());
    }

    public static async Task<Results<Ok<ApiDeviceDto>, NotFound>> GetDeviceByName(
        IDeviceService deviceService,
        string name)
    {
        var device = await deviceService.GetDeviceByNameAsync(name);

        if (device is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(device.ToDto());
    }

    public static async Task<CreatedAtRoute<ApiDeviceDto>> AddDevice(
        IDeviceService deviceService,
        ApiCreateDeviceDto createDTO)
    {
        var device = Device.FromDto(createDTO);
        await deviceService.CreateDeviceAsync(device);

        var deviceDTO = device.ToDto();

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetDeviceById),
            routeValues: new { deviceId = deviceDTO.Id.ToString() },
            value: deviceDTO
        );
    }

    public static async Task<Results<NotFound, NoContent>> UpdateDevice(
        IDeviceService deviceService,
        int deviceId,
        ApiUpdateDeviceDto updateDTO)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        device.Location = updateDTO.Location ?? device.Location;
        device.Name = updateDTO.Name ?? device.Name;
        device.Description = updateDTO.Description ?? device.Description;

        await deviceService.UpdateDeviceAsync(device);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteDevice(
        IDeviceService deviceService,
        int deviceId)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        await deviceService.DeleteDeviceAsync(device);

        return TypedResults.NoContent();
    }
}
