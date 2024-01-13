using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Devices;
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
    public async Task GetTemperatures_NoQueryParams_ReturnsOkAndTemperaturesInTimestampDesc()
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
            Timestamp = DateTime.UtcNow.AddDays(-1),
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
        temperatures.Should().HaveCount(2).And.SatisfyRespectively(
            latestDto =>
            {
                latestDto.Id.Should().Be(temperature2.Id);
                latestDto.DeviceId.Should().Be(device2.Id);
                latestDto.Timestamp.Should().Be(temperature2.Timestamp);
                latestDto.DegreesCelsius.Should().Be(temperature2.Value);
            },
            earliestDto =>
            {
                earliestDto.Id.Should().Be(temperature1.Id);
                earliestDto.DeviceId.Should().Be(device1.Id);
                earliestDto.Timestamp.Should().Be(temperature1.Timestamp);
                earliestDto.DegreesCelsius.Should().Be(temperature1.Value);
            }
        );
    }

    [Fact]
    public async Task GetTemperatures_WithDevicesQueryParams_ReturnsOkAndTemperaturesWithDeviceInTimestampDesc()
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
            Timestamp = DateTime.UtcNow.AddDays(-1),
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
        temperatures.Should().HaveCount(2).And.SatisfyRespectively(
            latestDto =>
            {
                latestDto.Id.Should().Be(temperature2.Id);
                latestDto.DeviceId.Should().Be(device2.Id);
                latestDto.Timestamp.Should().Be(temperature2.Timestamp);
                latestDto.DegreesCelsius.Should().Be(temperature2.Value);
                var deviceAssertion = latestDto.Device.Should().BeOfType<ApiDeviceDto>();
                deviceAssertion.Which.Id.Should().Be(device2.Id);
                deviceAssertion.Which.Name.Should().Be(device2.Name);
            },
            earliestDto =>
            {
                earliestDto.Id.Should().Be(temperature1.Id);
                earliestDto.DeviceId.Should().Be(device1.Id);
                earliestDto.Timestamp.Should().Be(temperature1.Timestamp);
                earliestDto.DegreesCelsius.Should().Be(temperature1.Value);
                var deviceAssertion = earliestDto.Device.Should().BeOfType<ApiDeviceDto>();
                deviceAssertion.Which.Id.Should().Be(device1.Id);
                deviceAssertion.Which.Name.Should().Be(device1.Name);
            }
        );
    }
}
