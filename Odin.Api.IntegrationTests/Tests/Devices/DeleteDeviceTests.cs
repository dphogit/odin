using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class DeleteDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public async Task InitializeAsync()
    {
        await factory.SeedDatabaseAsync(dbContext =>
        {
            dbContext.Devices.AddRange(
                new Device() { Id = 1, Name = "Device 1", Description = "Description 1", Location = "Location 1" }
            );
        });
    }

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Delete_DeviceInDb_ReturnsNoContentAndDeletesDevice()
    {
        // Act
        var response = await _httpClient.DeleteAsync("devices/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var device = await dbContext.Devices.FindAsync(1);
        device.Should().BeNull();
    }

    [Fact]
    public async Task Update_NoExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _httpClient.DeleteAsync("devices/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
