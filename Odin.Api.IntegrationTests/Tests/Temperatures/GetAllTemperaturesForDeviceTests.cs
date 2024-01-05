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
    public async Task GetTemperatures_DeviceInDb_ReturnsOkAndLast7DaysOfTemperaturesInAscTimestampOrder()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var today = DateTime.UtcNow;

        var temperature1 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = today.AddDays(-8),
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };
        var temperature2 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = today.AddDays(-7),
            Value = 26.9,
            UnitId = degreesCelsiusUnit.Id
        };
        var temperature3 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = today.AddDays(-6),
            Value = 27.1,
            UnitId = degreesCelsiusUnit.Id
        };
        await factory.InsertAsync(temperature1, temperature2, temperature3);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}/temperatures?days=7");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var temperatures = await response.Content.ReadFromJsonAsync<List<ApiTemperatureDto>>();
        temperatures.Should().HaveCount(2).And.Equal(
            [
                new ApiTemperatureDto
                {
                    Id = temperature2.Id,
                    DeviceId = device.Id,
                    Timestamp = temperature2.Timestamp,
                    DegreesCelsius = temperature2.Value,
                },
                new ApiTemperatureDto
                {
                    Id = temperature3.Id,
                    DeviceId = device.Id,
                    Timestamp = temperature3.Timestamp,
                    DegreesCelsius = temperature3.Value,
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
