using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Models;

namespace Odin.Api.Services;

public class UnitService(AppDbContext dbContext) : IUnitService
{
    public async Task<IEnumerable<Unit>> GetUnitsAsync()
    {
        return await dbContext.Units.ToListAsync();
    }

    public async Task<Unit?> GetUnitByNameAsync(string name)
    {
        return await dbContext.Units.SingleAsync(unit => unit.Name == name);
    }

    public async Task<Unit?> GetUnitByIdAsync(int id)
    {
        return await dbContext.Units.FindAsync(id);
    }
}
