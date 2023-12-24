using FluentAssertions;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace Odin.Api.IntegrationTests;

public sealed class MsSqlTests : IAsyncLifetime
{
    // Image should be consistent with docker-compose.yml
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => msSqlContainer.GetConnectionString();

    public Task InitializeAsync()
    {
        return msSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return msSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task HelloMsSql()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";

        var result = await command.ExecuteScalarAsync() as int?;
        result.GetValueOrDefault().Should().Be(1);
    }
}
