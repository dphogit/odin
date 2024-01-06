using System.Net;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class DeleteDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Delete_DeviceInDb_ReturnsNoContentAndDeletesDevice()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        // Act
        var response = await _httpClient.DeleteAsync($"devices/{device.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedDevice = await factory.FindAsync<Device>(device.Id);
        verifiedDevice.Should().BeNull();
    }

    [Fact]
    public async Task Delete_DeviceWithTemperatures_DeletesDeviceAndDependentTemperatures()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit() { Name = "Degrees Celsius", Symbol = "°C" };
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
        var response = await _httpClient.DeleteAsync($"devices/{device.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedDevice = await factory.FindAsync<Device>(device.Id);
        verifiedDevice.Should().BeNull();

        var verifiedTemperature = await factory.FindAsync<Temperature>(temperature.Id);
        verifiedTemperature.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        // Act
        var response = await _httpClient.DeleteAsync($"devices/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
