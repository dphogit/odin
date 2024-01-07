using Odin.Api.Models;

namespace Odin.Api.Services.Exceptions;

public class UnitHasAssociatedMeasurementsException(Unit unit)
    : InvalidOperationException($"Cannot delete unit \"{unit.Name}\" because it has associated measurements.")
{
    public Unit Unit { get; } = unit;
}
