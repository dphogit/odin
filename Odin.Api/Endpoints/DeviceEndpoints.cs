using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.DTOs;
using Odin.Api.Models;
using Odin.Api.Services;

namespace Odin.Api.Endpoints;

public static class DeviceEndpoints
{
    public static RouteGroupBuilder MapDeviceEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", GetDevices).WithName(nameof(GetDevices));
        builder.MapGet("/{id}", GetDeviceById).WithName(nameof(GetDeviceById));
        builder.MapPost("/", AddDevice).WithName(nameof(AddDevice));
        builder.MapPut("/{id}", UpdateDevice).WithName(nameof(UpdateDevice));
        builder.MapDelete("/{id}", DeleteDevice).WithName(nameof(DeleteDevice));
        return builder;
    }

    public static async Task<Ok<List<DeviceDTO>>> GetDevices(IDeviceService deviceService)
    {
        var devices = await deviceService.GetDevicesAsync();
        var deviceDTOs = devices.Select(device => device.ToDTO()).ToList();
        return TypedResults.Ok(deviceDTOs);
    }

    public static async Task<Results<Ok<DeviceDTO>, NotFound>> GetDeviceById(IDeviceService deviceService, int id)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(device.ToDTO());
    }

    public static async Task<CreatedAtRoute<DeviceDTO>> AddDevice(IDeviceService deviceService, CreateDeviceDTO createDTO)
    {
        var device = Device.FromDTO(createDTO);

        await deviceService.CreateDeviceAsync(device);

        var deviceDTO = device.ToDTO();

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetDeviceById),
            routeValues: new { id = deviceDTO.Id.ToString() },
            value: deviceDTO
        );
    }

    public static async Task<NoContent> UpdateDevice(IDeviceService deviceService, int id, UpdateDeviceDTO updateDTO)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NoContent();

        device.Location = updateDTO.Location ?? device.Location;
        device.Name = updateDTO.Name ?? device.Name;
        device.Description = updateDTO.Description ?? device.Description;

        await deviceService.UpdateDeviceAsync(device);

        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteDevice(IDeviceService deviceService, int id)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NoContent();

        await deviceService.DeleteDeviceAsync(device);

        return TypedResults.NoContent();
    }
}
