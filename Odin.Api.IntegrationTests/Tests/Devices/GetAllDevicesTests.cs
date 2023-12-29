using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class GetAllDevicesTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Get_PopulatedDb_ReturnsAllDevicesAndOk()
    {
        // Arrange
        var device1 = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        var device2 = new Device() { Name = "Device 2", Description = "Description 2", Location = "Location 2" };
        await factory.InsertAsync(device1, device2);

        var id1 = device1.Id;
        var id2 = device2.Id;

        // Act
        var response = await _httpClient.GetAsync("devices");
        var deviceDtos = await response.Content.ReadFromJsonAsync<List<ApiDeviceDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        deviceDtos.Should().NotBeNull().And.Satisfy(
            d => d.Id == id1 && d.Name == "Device 1" && d.Description == "Description 1" && d.Location == "Location 1",
            d => d.Id == id2 && d.Name == "Device 2" && d.Description == "Description 2" && d.Location == "Location 2"
        );
    }
}
