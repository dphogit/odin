using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class UpdateDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Update_DeviceInDb_ReturnsNoContentAndUpdatesDevice()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        ApiUpdateDeviceDto updateDeviceDTO = new()
        {
            Name = "Device 1 Updated",
            Description = "Description 1 Updated",
            Location = "Location 1 Updated"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"devices/{device.Id}", updateDeviceDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifiedDevice = await factory.FindAsync<Device>(device.Id);
        verifiedDevice.Should().BeEquivalentTo(updateDeviceDTO);
    }

    [Fact]
    public async Task Update_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        ApiUpdateDeviceDto updateDeviceDTO = new()
        {
            Name = "Device 1 Updated",
            Description = "Description 1 Updated",
            Location = "Location 1 Updated"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"devices/{id}", updateDeviceDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
