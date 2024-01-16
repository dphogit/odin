using Microsoft.AspNetCore.Http.HttpResults;
using Odin.Api.Config;
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
    /// <param name="page">
    ///     The current page to retrieve from for the collection request.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of records to retrieve per page.
    /// </param>
    public static async Task<Ok<PaginatedResponseSchema<ApiTemperatureDto>>> GetAllTemperatures(
        HttpContext httpContext,
        ITemperatureService temperatureService,
        bool withDevice = false,
        int page = 1,
        int limit = TemperatureConfig.DefaultPaginationLimit
    )
    {
        if (limit < 1)
            limit = TemperatureConfig.DefaultPaginationLimit;
        else if (limit > TemperatureConfig.MaxPaginationLimit)
            limit = TemperatureConfig.MaxPaginationLimit;

        var temperatures = page >= 1 ? await temperatureService.GetTemperaturesAsync(withDevice, page, limit) : [];
        var totalTemperatures = await temperatureService.CountTotalTemperaturesAsync();

        var temperatureDtos = temperatures.Select(t => t.ToDto(t.Device?.ToDto()));

        var responseMetaData = new ResponseMeta
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
