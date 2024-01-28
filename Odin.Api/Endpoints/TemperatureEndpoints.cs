using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Odin.Api.Endpoints.Pagination;
using Odin.Api.Models;
using Odin.Api.Services;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.Endpoints;

public static class TemperatureEndpoints
{
    public static RouteGroupBuilder MapTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", GetAllTemperatures).WithName(nameof(GetAllTemperatures));
        builder.MapGet("/{temperatureId}", GetTemperature).WithName(nameof(GetTemperature));
        builder.MapDelete("/{temperatureId}", DeleteTemperature).WithName(nameof(DeleteTemperature));
        return builder;
    }

    /// <summary>
    ///    Retrieves an array of temperatures, which are sorted by timestamp descending. Responses are paginated.
    /// </summary>
    /// <param name="withDevice">
    ///     If true, the device associated with the temperature will be included with each temperature in the response.
    /// </param>
    /// <param name="minValue">
    ///    The inclusive minimum value of the temperature to retrieve in degrees Celsius.
    ///    If not specified, no minimum value filter will be applied.
    /// </param>
    /// <param name="maxValue">
    ///   The inclusive maximum value of the temperature to retrieve in degrees Celsius.
    ///   If not specified, no maximum value filter will be applied.
    ///   </param>
    /// <param name="page">
    ///     The current page to retrieve from for the collection request.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of records to retrieve per page.
    /// </param>
    /// <param name="sort">
    ///    The sort order of results using the timestamp of the temperature taken. Valid values are "asc" and "desc".
    ///    Defaults to "desc" if not specified or an invalid value is provided.
    /// </param>
    /// <param name="deviceIds">
    ///    The device IDs of the devices to retrieve temperatures for. If not specified/empty, temperatures for
    ///    all devices will be retrieved. Note that the query key is actually `deviceId` mapped to
    ///    the `deviceIds` function parameter for clarity.
    /// </param>
    public static async Task<Ok<PaginatedResponseSchema<ApiTemperatureDto>>> GetAllTemperatures(
        HttpContext httpContext,
        ITemperatureService temperatureService,
        double? minValue,
        double? maxValue,
        [FromQuery(Name = "deviceId")] int[] deviceIds,
        bool withDevice = false,
        int page = 1,
        int limit = PaginationConstants.DefaultPaginationLimit,
        string sort = "desc"
    )
    {
        if (limit < 1)
            limit = PaginationConstants.DefaultPaginationLimit;
        else if (limit > PaginationConstants.MaxPaginationLimit)
            limit = PaginationConstants.MaxPaginationLimit;

        var timestampSort = sort.ToLower() switch
        {
            "asc" => TimestampSortOptions.Ascending,
            "desc" => TimestampSortOptions.Descending,
            _ => TimestampSortOptions.Descending
        };

        var options = new GetTemperatureOptions
        {
            WithDevice = withDevice,
            Page = page,
            Limit = limit,
            TimestampSort = timestampSort,
            MinValue = minValue,
            MaxValue = maxValue,
            DeviceIds = deviceIds.Length > 0 ? deviceIds : null
        };

        var temperatures = page >= 1 ? await temperatureService.GetTemperaturesAsync(options) : [];
        var totalTemperatures = page >= 1 ? await temperatureService.CountTotalTemperaturesAsync(options) : 0;

        var temperatureDtos = temperatures.Select(t => t.ToDto(t.Device?.ToDto()));

        var responseMetaData = new CollectionResponseMeta
        {
            Page = page,
            Limit = limit,
            Count = temperatureDtos.Count(),
            Total = totalTemperatures
        };

        var responseBody = new PaginatedResponseSchema<ApiTemperatureDto>
        {
            Data = temperatureDtos,
            Meta = responseMetaData,
        };

        return TypedResults.Ok(responseBody);
    }

    public static async Task<Results<Ok<ApiTemperatureDto>, NotFound>> GetTemperature(
        ITemperatureService temperatureService,
        int temperatureId)
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(temperature.ToDto());
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTemperature(
        ITemperatureService temperatureService,
        int temperatureId
    )
    {
        var temperature = await temperatureService.GetTemperatureByIdAsync(temperatureId);

        if (temperature is null)
            return TypedResults.NotFound();

        await temperatureService.DeleteTemperatureAsync(temperature);
        return TypedResults.NoContent();
    }
}
