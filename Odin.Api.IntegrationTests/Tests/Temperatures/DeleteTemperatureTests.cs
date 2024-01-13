using System.Net;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class DeleteTemperatureTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Delete_TemperatureInDb_ReturnsNoContentAndDeletesTemperature()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = DateTime.UtcNow,
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };
        await factory.InsertAsync(temperature);

        // Act
        var response = await _httpClient.DeleteAsync($"/temperatures/{temperature.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedTemperature = await factory.FindAsync<Temperature>(temperature.Id);
        verifiedTemperature.Should().BeNull();
    }

    [Fact]
    public async Task Delete_TemperatureNotExist_ReturnsNotFound()
    {
        // Arrange
        int temperatureId = 1;

        // Act
        var response = await _httpClient.DeleteAsync($"/temperatures/{temperatureId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
