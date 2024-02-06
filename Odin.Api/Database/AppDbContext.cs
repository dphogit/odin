using Microsoft.EntityFrameworkCore;
using Odin.Api.Models;

namespace Odin.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Unit> Units => Set<Unit>();

    // Table per concrete type (TPC) mapping strategy (using Measurement as root entity)
    public DbSet<Temperature> Temperatures => Set<Temperature>();

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Measurement>().UseTpcMappingStrategy();

        builder.Entity<Measurement>()
            .HasOne(m => m.Unit)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(builder);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is CreatedAtAndUpdatedAtEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (CreatedAtAndUpdatedAtEntity)entry.Entity;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            if (entry.State == EntityState.Added)
                entity.CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
