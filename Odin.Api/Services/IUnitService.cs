using Odin.Api.Models;

namespace Odin.Api.Services;

public interface IUnitService
{
    public Task<IEnumerable<Unit>> GetUnitsAsync();

    public Task<Unit?> GetUnitByIdAsync(int id);

    public Task<Unit?> GetUnitByNameAsync(string name);

    public Task CreateUnitAsync(Unit unit);

    public Task UpdateUnitAsync(Unit unit);

    public Task DeleteUnitAsync(Unit unit);
}
