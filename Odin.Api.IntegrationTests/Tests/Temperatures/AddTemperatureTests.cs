using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Temperatures;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class AddTemperatureTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Add_ValidBody_InsertsTemperatureIntoDbAndReturnsCreatedWithHeaderLocation()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        ApiAddTemperatureDto addTemperatureDto = new()
        {
            DeviceId = device.Id,
            Timestamp = DateTimeOffset.UtcNow,
            DegreesCelsius = 24.5
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("temperatures", addTemperatureDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var temperatureDto = await response.Content.ReadFromJsonAsync<ApiTemperatureDto>();
        temperatureDto.Should().BeOfType<ApiTemperatureDto>().Which.Id.Should().BeOfType(typeof(int));
        temperatureDto.Should().BeEquivalentTo(addTemperatureDto);
        response.Headers.Location.Should().BeOfType<Uri>()
            .Which.AbsolutePath.Should().Be($"/temperatures/{temperatureDto!.Id}");
    }
}