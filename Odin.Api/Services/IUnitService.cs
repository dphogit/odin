using Odin.Api.Models;

namespace Odin.Api.Services;

/// <summary>
///   A service for performing read-only operations on unit entities in the database.
/// </summary>
public interface IUnitService
{
    public Task<IEnumerable<Unit>> GetUnitsAsync();

    public Task<Unit?> GetUnitByIdAsync(int id);

    public Task<Unit?> GetUnitByNameAsync(string name);
}
