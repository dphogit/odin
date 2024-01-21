using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.Endpoints.Pagination;
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

        var responseJson = await response.Content.ReadFromJsonAsync<PaginatedResponseSchema<ApiTemperatureDto>>();
        responseJson.Should().NotBeNull();

        var meta = responseJson!.Meta;
        var temperatures = responseJson!.Data;

        meta.Page.Should().Be(1);
        meta.Limit.Should().Be(PaginationConstants.DefaultPaginationLimit);
        meta.Count.Should().Be(2);
        meta.Total.Should().Be(2);

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

        var responseJson = await response.Content.ReadFromJsonAsync<PaginatedResponseSchema<ApiTemperatureDto>>();
        responseJson.Should().NotBeNull();

        var meta = responseJson!.Meta;
        var temperatures = responseJson!.Data;

        meta.Page.Should().Be(1);
        meta.Limit.Should().Be(PaginationConstants.DefaultPaginationLimit);
        meta.Count.Should().Be(2);
        meta.Total.Should().Be(2);

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

    [Fact]
    public async Task GetTemperatures_WithPageAndLimit_ReturnsFirstPageResponse()
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
        var response = await _httpClient.GetAsync("temperatures?page=1&limit=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseJson = await response.Content.ReadFromJsonAsync<PaginatedResponseSchema<ApiTemperatureDto>>();
        responseJson.Should().NotBeNull();

        var meta = responseJson!.Meta;
        var temperatures = responseJson!.Data;

        meta.Page.Should().Be(1);
        meta.Limit.Should().Be(1);
        meta.Count.Should().Be(1);
        meta.Total.Should().Be(2);

        temperatures.Should().HaveCount(1).And.SatisfyRespectively(
            dto =>
            {
                dto.Id.Should().Be(temperature2.Id);
                dto.DeviceId.Should().Be(device2.Id);
                dto.Timestamp.Should().Be(temperature2.Timestamp);
                dto.DegreesCelsius.Should().Be(temperature2.Value);
            }
        );
    }

    [Fact]
    public async Task GetTemperatures_WithPageAndLimit_ReturnsLastPageResponse()
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
        var response = await _httpClient.GetAsync("temperatures?page=2&limit=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseJson = await response.Content.ReadFromJsonAsync<PaginatedResponseSchema<ApiTemperatureDto>>();
        responseJson.Should().NotBeNull();

        var meta = responseJson!.Meta;
        var temperatures = responseJson!.Data;

        meta.Page.Should().Be(2);
        meta.Limit.Should().Be(1);
        meta.Count.Should().Be(1);
        meta.Total.Should().Be(2);

        temperatures.Should().HaveCount(1).And.SatisfyRespectively(
            dto =>
            {
                dto.Id.Should().Be(temperature1.Id);
                dto.DeviceId.Should().Be(device1.Id);
                dto.Timestamp.Should().Be(temperature1.Timestamp);
                dto.DegreesCelsius.Should().Be(temperature1.Value);
            }
        );
    }

    [Fact]
    public async Task GetTemperatures_WithOutOfRangePage_ReturnsOkWithEmptyDataResponse()
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
        var response = await _httpClient.GetAsync("temperatures?page=3&limit=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseJson = await response.Content.ReadFromJsonAsync<PaginatedResponseSchema<ApiTemperatureDto>>();
        responseJson.Should().NotBeNull();

        var meta = responseJson!.Meta;
        var temperatures = responseJson!.Data;

        meta.Page.Should().Be(3);
        meta.Limit.Should().Be(1);
        meta.Count.Should().Be(0);
        meta.Total.Should().Be(2);

        temperatures.Should().BeEmpty();
    }
}
