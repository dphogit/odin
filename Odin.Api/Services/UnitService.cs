using Microsoft.EntityFrameworkCore;
using Odin.Api.Database;
using Odin.Api.Models;
using Odin.Api.Services.Exceptions;

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

    public async Task CreateUnitAsync(Unit unit)
    {
        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateUnitAsync(Unit unit)
    {
        dbContext.Units.Update(unit);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteUnitAsync(Unit unit)
    {
        var hasAssociatedMeasurements = await dbContext.Temperatures.AnyAsync(t => t.UnitId == unit.Id);

        if (hasAssociatedMeasurements)
            throw new UnitHasAssociatedMeasurementsException(unit);

        dbContext.Units.Remove(unit);
        await dbContext.SaveChangesAsync();
    }
}
