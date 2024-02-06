using Microsoft.EntityFrameworkCore;

namespace Odin.Api.Database;

public class DataSeeder(AppDbContext dbContext)
{
    public void Seed()
    {
        using var transaction = dbContext.Database.BeginTransaction();
        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Units ON");

        dbContext.Units.AddRange(Units.AllUnits);

        dbContext.SaveChanges();

        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Units OFF");
        transaction.Commit();
    }
}
