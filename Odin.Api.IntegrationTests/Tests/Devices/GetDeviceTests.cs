using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.DTOs;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class GetDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public async Task InitializeAsync()
    {
        // Arrange
        await factory.SeedDatabaseAsync(dbContext =>
        {
            dbContext.Devices.AddRange(
                new Device() { Id = 1, Name = "Device 1", Description = "Description 1", Location = "Location 1" }
            );
        });
    }

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task GetById_PopulatedDb_ReturnsDeviceAndOk()
    {
        // Act
        var response = await _httpClient.GetAsync("devices/1");
        var deviceDTO = await response.Content.ReadFromJsonAsync<DeviceDTO>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        deviceDTO.Should().BeOfType<DeviceDTO>().Which.Should().BeEquivalentTo(
            new DeviceDTO() { Id = 1, Name = "Device 1", Description = "Description 1", Location = "Location 1" },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _httpClient.GetAsync("devices/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
