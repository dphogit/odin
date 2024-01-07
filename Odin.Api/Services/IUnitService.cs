using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IUnitService
{
    public Task<IEnumerable<Unit>> GetUnitsAsync();

    public Task<Unit?> GetUnitByIdAsync(int id);

    public Task<Unit?> GetUnitByNameAsync(string name);

    public Task CreateUnitAsync(Unit unit);

    public Task UpdateUnitAsync(Unit unit);

    /// <summary>
    ///     Deletes a unit if it has no associated measurements, otherwise throws a
    ///     <see cref="UnitHasAssociatedMeasurementsException"/>.
    /// </summary>
    /// <exception cref="UnitHasAssociatedMeasurementsException" />
    public Task DeleteUnitAsync(Unit unit);
}
