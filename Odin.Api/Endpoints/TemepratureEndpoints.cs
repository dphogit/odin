namespace Odin.Api.Endpoints;

public static class TemperatureEndpoints
{
    public static RouteGroupBuilder MapTemperatureEndpoints(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", AddTemperature).WithName(nameof(AddTemperature));
        return builder;
    }

    public static IResult AddTemperature()
    {
        Console.WriteLine("Received POST request.");
        return TypedResults.Created();
    }
}
