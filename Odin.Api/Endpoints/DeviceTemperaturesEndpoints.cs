using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Config;
using Odin.Api.Endpoints.ResponseSchemas;
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
        builder.MapGet("/time-series", GetTimeSeriesDataForDevice).WithName(nameof(GetTimeSeriesDataForDevice));
        builder.MapPost("/", AddTemperatureForDevice).WithName(nameof(AddTemperatureForDevice));
        return builder;
    }

    public static async Task<Results<Ok<IEnumerable<TimeSeriesDataPoint>>, NotFound, BadRequest<string>>>
        GetTimeSeriesDataForDevice(
            ITemperatureService temperatureService,
            IDeviceService deviceService,
            HttpRequest httpRequest,
            int deviceId,
            string timeRange = "month")
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        TimeRange range;
        switch (timeRange.ToLower().Trim())
        {
            case "year":
                range = TimeRange.Year;
                break;
            case "month":
                range = TimeRange.Month;
                break;
            case "week":
                range = TimeRange.Week;
                break;
            default:
                return TypedResults.BadRequest("Invalid time range. Valid values are \"year\", \"month\", \"week\".");
        }

        // Timezone offset will bucket data points according to the user's timezone, defaults to UTC.
        var timezoneOffset = httpRequest.GetTimezoneOffset();

        var timeSeriesData = await temperatureService.GetTimeSeriesDataForDeviceAsync(deviceId, range, timezoneOffset);
        return TypedResults.Ok(timeSeriesData);
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
