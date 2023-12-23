using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Database;
using Odin.Api.DTOs;
using Odin.Api.Models;

namespace Odin.Api.Endpoints;

public static class DeviceEndpoints
{
    public static RouteGroupBuilder MapDeviceEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/{id}", GetDevice).WithName("GetDevice");
        builder.MapPost("/", AddDevice).WithName("AddDevice");
        return builder;
    }

    public static async Task<Results<Ok<DeviceDTO>, NotFound>> GetDevice(AppDbContext db, Guid id)
    {
        var device = await db.Devices.FindAsync(id);

        if (device is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(device.ToDTO());
    }

    public static async Task<CreatedAtRoute<DeviceDTO>> AddDevice(AppDbContext db, CreateDeviceDTO createDTO)
    {
        var device = Device.FromDTO(createDTO);

        db.Devices.Add(device);
        await db.SaveChangesAsync();

        var deviceDTO = device.ToDTO();

        return TypedResults.CreatedAtRoute(
            routeName: "GetDevice",
            routeValues: new { id = deviceDTO.Id.ToString() },
            value: deviceDTO
        );
    }
}
