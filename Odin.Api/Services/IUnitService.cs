using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IUnitService
{
    Task<IEnumerable<Unit>> GetUnitsAsync();
    Task<Unit?> GetUnitByIdAsync(int id);
    Task CreateUnitAsync(Unit unit);
    Task UpdateUnitAsync(Unit unit);
    Task DeleteUnitAsync(Unit unit);
}
