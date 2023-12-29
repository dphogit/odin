using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class GetDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task GetById_PopulatedDb_ReturnsDeviceAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        using var scope = factory.ScopeFactory.CreateScope();
        var id = scope.ServiceProvider.GetRequiredService<AppDbContext>().Devices.Single().Id;

        // Act
        var response = await _httpClient.GetAsync($"devices/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDTO = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDTO.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1"
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        // Act
        var response = await _httpClient.GetAsync($"devices/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByName_ExistingDevice_ReturnsDeviceAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        // Act
        var response = await _httpClient.GetAsync($"devices/name/{device.Name}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDTO = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDTO.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = device.Id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1"
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetByName_NonExistentName_ReturnsNotFound()
    {
        // Arrange
        string name = "Device 1";

        // Act
        var response = await _httpClient.GetAsync($"devices/name/{name}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
