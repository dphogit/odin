using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class CreateDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Create_ValidBody_InsertsDeviceIntoDbAndReturnsCreatedWithHeaderLocation()
    {
        // Arrange
        ApiCreateDeviceDto createDeviceDTO = new()
        {
            Name = "Device 1",
            Description = "Description 1",
            Location = "Location 1"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("devices", createDeviceDTO);
        var deviceDTO = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        deviceDTO.Should().BeOfType<ApiDeviceDto>().Which.Id.Should().BeOfType(typeof(int));
        deviceDTO.Should().BeEquivalentTo(createDeviceDTO);
        response.Headers.Location.Should().BeOfType<Uri>()
            .Which.AbsolutePath.Should().Be($"/devices/{deviceDTO!.Id}");
    }
}
