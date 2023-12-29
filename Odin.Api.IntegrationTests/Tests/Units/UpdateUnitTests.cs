using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Units;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class UpdateUnitTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Update_UnitInDb_ReturnsNoContentAndUpdatesUnit()
    {
        // Arrange
        var unit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(unit);

        ApiUpdateUnitDto updateUnitDTO = new()
        {
            Name = "Degrees Fahrenheit",
            Symbol = "°F"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"units/{unit.Id}", updateUnitDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedUnit = await factory.FindAsync<Unit>(unit.Id);
        verifiedUnit.Should().BeEquivalentTo(updateUnitDTO);
    }

    [Fact]
    public async Task Update_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        ApiUpdateUnitDto updateUnitDTO = new()
        {
            Name = "Degrees Fahrenheit",
            Symbol = "°F"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"units/{id}", updateUnitDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
