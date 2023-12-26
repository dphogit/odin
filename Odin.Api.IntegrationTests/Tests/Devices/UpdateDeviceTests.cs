using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
using Odin.Api.DTOs;
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

        using var createScope = factory.ScopeFactory.CreateScope();
        var id = createScope.ServiceProvider.GetRequiredService<AppDbContext>().Devices.Single().Id;

        UpdateDeviceDTO updateDeviceDTO = new()
        {
            Name = "Device 1 Updated",
            Description = "Description 1 Updated",
            Location = "Location 1 Updated"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"devices/{id}", updateDeviceDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var verifyScope = factory.Services.CreateScope();
        var verifiedDevice = await verifyScope.ServiceProvider.GetRequiredService<AppDbContext>().Devices.FindAsync(id);
        verifiedDevice.Should().BeEquivalentTo(updateDeviceDTO);
    }

    [Fact]
    public async Task Update_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        UpdateDeviceDTO updateDeviceDTO = new()
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
