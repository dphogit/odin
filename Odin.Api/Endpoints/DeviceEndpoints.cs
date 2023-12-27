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
        builder.MapGet("/{id}", GetDeviceById).WithName(nameof(GetDeviceById));
        builder.MapPost("/", AddDevice).WithName(nameof(AddDevice));
        builder.MapPut("/{id}", UpdateDevice).WithName(nameof(UpdateDevice));
        builder.MapDelete("/{id}", DeleteDevice).WithName(nameof(DeleteDevice));
        return builder;
    }

    public static async Task<Ok<List<ApiDeviceDto>>> GetDevices(IDeviceService deviceService)
    {
        var devices = await deviceService.GetDevicesAsync();
        var deviceDTOs = devices.Select(device => device.ToDTO()).ToList();
        return TypedResults.Ok(deviceDTOs);
    }

    public static async Task<Results<Ok<ApiDeviceDto>, NotFound>> GetDeviceById(IDeviceService deviceService, int id)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(device.ToDTO());
    }

    public static async Task<CreatedAtRoute<ApiDeviceDto>> AddDevice(IDeviceService deviceService, ApiCreateDeviceDto createDTO)
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

    public static async Task<Results<NotFound, NoContent>> UpdateDevice(
        IDeviceService deviceService,
        int id,
        ApiUpdateDeviceDto updateDTO)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NotFound();

        device.Location = updateDTO.Location ?? device.Location;
        device.Name = updateDTO.Name ?? device.Name;
        device.Description = updateDTO.Description ?? device.Description;

        await deviceService.UpdateDeviceAsync(device);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteDevice(IDeviceService deviceService, int id)
    {
        var device = await deviceService.GetDeviceByIdAsync(id);

        if (device is null)
            return TypedResults.NotFound();

        await deviceService.DeleteDeviceAsync(device);

        return TypedResults.NoContent();
    }
}
