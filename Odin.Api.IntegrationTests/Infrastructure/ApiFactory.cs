using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Config;
using Odin.Api.Database;
using Respawn;
using Testcontainers.MsSql;
using Xunit;

namespace Odin.Api.IntegrationTests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    // Image should be consistent with compose.yaml
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public IServiceScopeFactory ScopeFactory { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    public string ConnectionString => _msSqlContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(ConnectionString));

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        });

        builder.UseTestingEnvironment();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        HttpClient = CreateClient();

        ScopeFactory = Services.GetRequiredService<IServiceScopeFactory>();

        await InitializeDbRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task ExecuteDbContextAsync(Action<AppDbContext> action)
    {
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        action(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public async Task InsertAsync<TEntity>(params TEntity[] entities) where TEntity : class
    {
        await ExecuteDbContextAsync(async dbContext => await dbContext.AddRangeAsync(entities));
    }

    public async Task<TEntity?> FindAsync<TEntity>(int id) where TEntity : class
    {
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.FindAsync<TEntity>(id);
    }

    public void SeedDb()
    {
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = new DataSeeder(dbContext);
        seeder.Seed();
    }

    private async Task InitializeDbRespawner()
    {
        _dbConnection = new SqlConnection(ConnectionString);
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection);
    }
}
