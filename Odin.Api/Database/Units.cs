using Odin.Api.Models;

namespace Odin.Api.Database;

public static class Units
{
    public static Unit DegreesCelsius { get; private set; } = new() { Id = 1, Name = "Degrees Celsius", Symbol = "°C" };

    /// <summary>
    ///     Returns the unit associated with the given measurement type.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     Thrown if the given measurement type is not supported or invalid.
    /// </exception>
    public static Unit GetAssociatedUnit(Measurement measurement)
    {
        return measurement switch
        {
            Temperature _ => DegreesCelsius,
            _ => throw new ArgumentException($"Measurement type {measurement.GetType()} is not supported"),
        };
    }

    public static IEnumerable<Unit> AllUnits =>
    [
        DegreesCelsius
    ];
}
