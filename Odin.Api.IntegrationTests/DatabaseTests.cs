using FluentAssertions;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace Odin.Api.IntegrationTests;

public sealed class DatabaseTests : IDisposable
{
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public DatabaseTests()
    {
        msSqlContainer.StartAsync().Wait();
    }

    public async void Dispose()
    {
        await msSqlContainer.DisposeAsync();
    }

    /// <summary> A "Hello World" test to ensure database connection is working.</summary>
    [Fact]
    public async Task Select1_ConnectionEstablished_Returns1()
    {
        await using var connection = new SqlConnection(msSqlContainer.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";

        var result = await command.ExecuteScalarAsync() as int?;
        result.GetValueOrDefault().Should().Be(1);
    }
}
