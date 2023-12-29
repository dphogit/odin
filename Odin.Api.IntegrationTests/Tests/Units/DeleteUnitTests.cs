using System.Net;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class DeleteUnitTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Delete_UnitInDb_ReturnsNoContentAndDeletesUnit()
    {
        // Arrange
        var unit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(unit);

        // Act
        var response = await _httpClient.DeleteAsync($"units/{unit.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedUnit = await factory.FindAsync<Unit>(unit.Id);
        verifiedUnit.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        // Act
        var response = await _httpClient.DeleteAsync($"units/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
