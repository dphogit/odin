using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class TemperatureService(AppDbContext dbContext, IUnitService unitService) : ITemperatureService
{
    public async Task AddTemperatureAsync(Temperature temperature)
    {
        var degreesCelsiusUnit = await unitService.GetUnitByNameAsync("Degrees Celsius") ??
                                 throw new TemperatureServiceException("Degrees Celsius unit not found");

        temperature.Unit = degreesCelsiusUnit;
        dbContext.Temperatures.Add(temperature);
        await dbContext.SaveChangesAsync();
    }
}

public class TemperatureServiceException(string message) : Exception(message) { }
