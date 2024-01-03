using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Shared.ApiDtos.Devices;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;
using Odin.Shared.ApiDtos.Temperatures;

namespace Odin.Api.IntegrationTests.Tests.Devices;

[Collection(nameof(ApiCollection))]
public class GetDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task GetById_PopulatedDb_ReturnsDeviceAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDTO = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDTO.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = device.Id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1"
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetById_WithTemperaturesSearchQueryTrue_ReturnsDeviceWithTemperaturesAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature = new Temperature() { DeviceId = device.Id, Value = 25.5, UnitId = degreesCelsiusUnit.Id };
        await factory.InsertAsync(temperature);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}?withTemperatures=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDto = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDto.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = device.Id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1",
                Temperatures = new List<ApiTemperatureDto>()
                {
                    new()
                    {
                        Id = temperature.Id,
                        DeviceId = device.Id,
                        Timestamp = temperature.Timestamp,
                        DegreesCelsius = temperature.Value,
                    }
                }
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetById_WithTemperaturesSearchQueryFalse_ReturnsDeviceOnlyAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var temperature = new Temperature() { DeviceId = device.Id, Value = 25.5, UnitId = degreesCelsiusUnit.Id };
        await factory.InsertAsync(temperature);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}?withTemperatures=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDto = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDto.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = device.Id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1",
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        int id = 1;

        // Act
        var response = await _httpClient.GetAsync($"devices/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByName_ExistingDevice_ReturnsDeviceAndOk()
    {
        // Arrange
        var device = new Device() { Name = "Device 1", Description = "Description 1", Location = "Location 1" };
        await factory.InsertAsync(device);

        // Act
        var response = await _httpClient.GetAsync($"devices/name/{device.Name}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deviceDTO = await response.Content.ReadFromJsonAsync<ApiDeviceDto>();
        deviceDTO.Should().BeOfType<ApiDeviceDto>().Which.Should().BeEquivalentTo(
            new ApiDeviceDto()
            {
                Id = device.Id,
                Name = "Device 1",
                Description = "Description 1",
                Location = "Location 1"
            },
            (options) => options.Excluding(dto => dto.CreatedAt).Excluding(dto => dto.UpdatedAt)
        );
    }

    [Fact]
    public async Task GetByName_NonExistentName_ReturnsNotFound()
    {
        // Arrange
        string name = "Device 1";

        // Act
        var response = await _httpClient.GetAsync($"devices/name/{name}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
