using FluentAssertions;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Odin.Api.IntegrationTests;

public sealed class DatabaseTests(MsSqlTests fixture) : IClassFixture<MsSqlTests>
{
    private readonly string connectionString = fixture.ConnectionString;

    /// <summary> A "Hello World" test to ensure database connection is working.</summary>
    [Fact]
    public async Task Select1_ConnectionEstablished_Returns1()
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";

        var result = await command.ExecuteScalarAsync() as int?;
        result.GetValueOrDefault().Should().Be(1);
    }
}
