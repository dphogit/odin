using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Temperatures;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class GetAllTemperaturesForDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task GetTemperatures_DeviceInDb_ReturnsOkAndTemperaturesBelongingToDeviceOnly()
    {
        // Arrange
        var device1 = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        var device2 = new Device { Name = "Raspberry Pi Pico Internal Temperature Wifi" };
        await factory.InsertAsync(device1, device2);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature1 = new Temperature()
        {
            DeviceId = device1.Id,
            Timestamp = DateTime.UtcNow,
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };
        var temperature2 = new Temperature()
        {
            DeviceId = device2.Id,
            Timestamp = DateTime.UtcNow,
            Value = 26.9,
            UnitId = degreesCelsiusUnit.Id
        };
        await factory.InsertAsync(temperature1, temperature2);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device1.Id}/temperatures");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var temperatures = await response.Content.ReadFromJsonAsync<List<ApiTemperatureDto>>();
        temperatures.Should().HaveCount(1).And.BeEquivalentTo(
            [
                new ApiTemperatureDto
                {
                    Id = temperature1.Id,
                    DeviceId = device1.Id,
                    Timestamp = temperature1.Timestamp,
                    DegreesCelsius = temperature1.Value,
                }
            ]
        );
    }

    [Fact]
    public async Task GetTemperatures_NoExistingDeviceId_ReturnsNotFound()
    {
        // Arrange
        int deviceId = 1;

        // Act
        var response = await _httpClient.GetAsync($"devices/{deviceId}/temperatures");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}