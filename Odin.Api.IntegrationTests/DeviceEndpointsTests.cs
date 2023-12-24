using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests;

public sealed class DeviceEndpointsTests : IClassFixture<MsSqlTests>, IDisposable
{
    private readonly Func<Task> resetDatabase;
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public DeviceEndpointsTests(MsSqlTests fixture)
    {
        resetDatabase = fixture.ResetDatabase;
        factory = new MsSqlWebApplicationFactory(fixture);
        client = factory.CreateClient();
    }

    public void Dispose()
    {
        factory.Dispose();
    }

    [Fact]
    public async Task GetDevices_InDb_ReturnsDevices()
    {
        // Arrange
        await resetDatabase();
        Device device1 = new() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        Device device2 = new() { Name = "Device 2", Description = "Description 2", Location = "Location 2" };

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Devices.AddRangeAsync(device1, device2);
            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync("devices");

        // Assert
        response.Should().Be200Ok().And.BeAs(new[] { device1, device2 });
    }

    [Fact]
    public async Task GetDevice_InDb_ReturnsDevice()
    {
        // Arrange
        await resetDatabase();
        Device device = new() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Devices.AddAsync(device);
            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync($"devices/{device.Id}");

        // Assert
        response.Should().Be200Ok().And.BeAs(device);
    }
}
