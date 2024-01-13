using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Temperatures;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class GetAllTemperaturesTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task GetTemperatures_NoQueryParams_ReturnsOkAndTemperatures()
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
        var response = await _httpClient.GetAsync("temperatures");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var temperatures = await response.Content.ReadFromJsonAsync<List<ApiTemperatureDto>>();
        temperatures.Should().HaveCount(2).And.Satisfy(
            dto => dto.Id == temperature1.Id
                && dto.DeviceId == device1.Id
                && dto.Timestamp == temperature1.Timestamp
                && dto.DegreesCelsius == temperature1.Value,
            dto => dto.Id == temperature2.Id
                && dto.DeviceId == device2.Id
                && dto.Timestamp == temperature2.Timestamp
                && dto.DegreesCelsius == temperature2.Value
        );
    }

    [Fact]
    public async Task GetTemperatures_WithDevicesQueryParams_ReturnsOkAndTemperaturesWithJoinedDevice()
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
        var response = await _httpClient.GetAsync("temperatures?withDevice=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var temperatures = await response.Content.ReadFromJsonAsync<List<ApiTemperatureDto>>();
        temperatures.Should().HaveCount(2).And.Satisfy(
            dto => dto.Id == temperature1.Id
                && dto.DeviceId == device1.Id
                && dto.Timestamp == temperature1.Timestamp
                && dto.DegreesCelsius == temperature1.Value
                && dto.Device != null
                && dto.Device.Id == device1.Id
                && dto.Device.Name == device1.Name,
            dto => dto.Id == temperature2.Id
                && dto.DeviceId == device2.Id
                && dto.Timestamp == temperature2.Timestamp
                && dto.DegreesCelsius == temperature2.Value
                && dto.Device != null
                && dto.Device.Id == device2.Id
                && dto.Device.Name == device2.Name
        );
    }
}
