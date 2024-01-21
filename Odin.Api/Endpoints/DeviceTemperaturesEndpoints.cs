using Microsoft.AspNetCore.Http.HttpResults;
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
        builder.MapGet("/", GetAllDeviceTemperatures).WithName(nameof(GetAllDeviceTemperatures));
        builder.MapGet("/time-series", GetTimeSeriesDataForDevice).WithName(nameof(GetTimeSeriesDataForDevice));
        builder.MapPost("/", AddTemperatureForDevice).WithName(nameof(AddTemperatureForDevice));
        return builder;
    }

    // TODO Needs to become paginated, create a new endpoint for obtaining time series data.
    public static async Task<Results<Ok<IEnumerable<ApiTemperatureDto>>, NotFound>> GetAllDeviceTemperatures(
        ITemperatureService temperatureService,
        IDeviceService deviceService,
        int deviceId,
        int days = 30)
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        var temperatures = await temperatureService.GetTemperaturesForDeviceAsync(deviceId, days);
        var temperatureDtos = temperatures.Select(t => t.ToDto());
        return TypedResults.Ok(temperatureDtos);
    }

    public static async Task<Results<Ok<IEnumerable<TimeSeriesDataPoint>>, NotFound, BadRequest<string>>>
        GetTimeSeriesDataForDevice(
            ITemperatureService temperatureService,
            IDeviceService deviceService,
            HttpRequest httpRequest,
            int deviceId,
            string timeRange = "last30days")
    {
        var device = await deviceService.GetDeviceByIdAsync(deviceId);

        if (device is null)
            return TypedResults.NotFound();

        TimeRange range;
        switch (timeRange.ToLower().Trim())
        {
            case "last30days":
                range = TimeRange.Last30Days;
                break;
            case "last7days":
                range = TimeRange.Last7Days;
                break;
            default:
                return TypedResults.BadRequest("Invalid time range. Valid values are \"last30Days\", \"last7Days\".");
        }

        // Get timezone offset which will bucket data points according to the user's timezone, defaults to UTC.
        var timezoneOffset = TimeSpan.FromMinutes(0);
        if (httpRequest.Headers.TryGetValue("X-Timezone-Offset", out var values))
        {
            if (int.TryParse(values, out var offsetMinutes))
            {
                timezoneOffset = TimeSpan.FromMinutes(offsetMinutes);
            }
        }

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
