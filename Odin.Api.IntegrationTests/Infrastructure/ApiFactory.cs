using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(ConnectionString));

            // Skips migrations and immediately setups the database (if not already) with latest schema.
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        });
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

    public async Task ExecuteDbContextAsync(Action<AppDbContext> seedAction)
    {
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        seedAction(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public async Task InsertAsync<TEntity>(params TEntity[] entities) where TEntity : class
    {
        await ExecuteDbContextAsync(async dbContext => await dbContext.AddRangeAsync(entities));
    }

    private async Task InitializeDbRespawner()
    {
        _dbConnection = new SqlConnection(ConnectionString);
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection);
    }
}
