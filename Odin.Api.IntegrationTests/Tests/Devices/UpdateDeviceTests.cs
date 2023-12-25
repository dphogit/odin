using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

    public async Task InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        using var transaction = await dbContext.Database.BeginTransactionAsync();
        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Devices ON");
        await dbContext.Devices.AddRangeAsync(
            new Device() { Id = 1, Name = "Device 1", Description = "Description 1", Location = "Location 1" }
        );
        await dbContext.SaveChangesAsync();
        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Devices OFF");
        await transaction.CommitAsync();
    }

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Update_DeviceInDb_ReturnsNoContentAndUpdatesDevice()
    {
        // Arrange
        UpdateDeviceDTO updateDeviceDTO = new()
        {
            Name = "Device 1 Updated",
            Description = "Description 1 Updated",
            Location = "Location 1 Updated"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync("devices/1", updateDeviceDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var device = await dbContext.Devices.FindAsync(1);
        device.Should().BeEquivalentTo(updateDeviceDTO);
    }

    [Fact]
    public async Task Update_NoExistingId_ReturnsNotFound()
    {
        // Arrange
        UpdateDeviceDTO updateDeviceDTO = new()
        {
            Name = "Device 1 Updated",
            Description = "Description 1 Updated",
            Location = "Location 1 Updated"
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync("devices/3", updateDeviceDTO);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
