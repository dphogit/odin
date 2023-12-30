using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Temperatures;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class GetTemperatureForDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task GetTemperatureById_BelongsToExistingDevice_ReturnsOkWithTemperature()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature = new Temperature
        {
            DeviceId = device.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };
        await factory.InsertAsync(temperature);

        // Act
        var response = await _httpClient.GetAsync($"/devices/{device.Id}/temperatures/{temperature.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var temperatureDto = await response.Content.ReadFromJsonAsync<ApiTemperatureDto>();
        temperatureDto.Should().BeOfType<ApiTemperatureDto>().Which.Id.Should().Be(temperature.Id);
        temperatureDto.Should().BeEquivalentTo(new ApiTemperatureDto
        {
            Id = temperature.Id,
            DeviceId = device.Id,
            Timestamp = temperature.Timestamp,
            DegreesCelsius = temperature.Value
        });
    }

    [Fact]
    public async Task GetTemperatureById_DeviceExistsWithNoTemperatures_ReturnsNotFound()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var temperatureId = 1;

        // Act
        var response = await _httpClient.GetAsync($"/devices/{device.Id}/temperatures/{temperatureId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTemperatureById_NotBelongToTargetDevice_ReturnsNotFound()
    {
        // Arrange
        var device1 = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        var device2 = new Device { Name = "Raspberry Pi Pico Internal Temperature Wifi" };
        await factory.InsertAsync(device1, device2);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature1 = new Temperature
        {
            DeviceId = device1.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };
        await factory.InsertAsync(temperature1);

        // Act
        var response = await _httpClient.GetAsync($"/devices/{device2.Id}/temperatures/{temperature1.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTemperatureById_NoExistingDeviceId_ReturnsNotFound()
    {
        // Arrange
        var deviceId = 1;
        var temperatureId = 1;

        // Act
        var response = await _httpClient.GetAsync($"/devices/{deviceId}/temperatures/{temperatureId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
