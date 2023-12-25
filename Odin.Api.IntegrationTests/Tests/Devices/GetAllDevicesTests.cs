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
public class GetAllDevicesTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public async Task InitializeAsync()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        using var transaction = await dbContext.Database.BeginTransactionAsync();
        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Devices ON");
        await dbContext.Devices.AddRangeAsync(
            new Device() { Id = 1, Name = "Device 1", Description = "Description 1", Location = "Location 1" },
            new Device() { Id = 2, Name = "Device 2", Description = "Description 2", Location = "Location 2" }
        );
        await dbContext.SaveChangesAsync();
        dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Devices OFF");
        await transaction.CommitAsync();
    }

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Get_PopulatedDb_ReturnsAllDevicesAndOk()
    {
        // Act
        var response = await _httpClient.GetAsync("devices");
        var deviceDtos = await response.Content.ReadFromJsonAsync<List<DeviceDTO>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        deviceDtos.Should().NotBeNull().And.Satisfy(
            d => d.Id == 1 && d.Name == "Device 1" && d.Description == "Description 1" && d.Location == "Location 1",
            d => d.Id == 2 && d.Name == "Device 2" && d.Description == "Description 2" && d.Location == "Location 2"
        );
    }
}
