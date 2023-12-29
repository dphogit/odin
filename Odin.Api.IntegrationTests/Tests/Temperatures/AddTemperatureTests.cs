using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
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

        using var scope = factory.ScopeFactory.CreateScope();
        var deviceId = scope.ServiceProvider.GetRequiredService<AppDbContext>().Devices
            .Single(d => d.Name == "Arduino Uno R3 TMP36 Button Serial").Id;

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        ApiAddTemperatureDto addTemperatureDto = new()
        {
            DeviceId = deviceId,
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
