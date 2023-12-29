using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Units;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Units;

[Collection(nameof(ApiCollection))]
public class GetUnitTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task GetById_PopulatedDb_ReturnsDeviceAndOk()
    {
        // Arrange
        var unit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(unit);

        // Act
        var response = await _httpClient.GetAsync($"units/{unit.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var unitDto = await response.Content.ReadFromJsonAsync<ApiUnitDto>();
        unitDto.Should().BeOfType<ApiUnitDto>().Which.Should().BeEquivalentTo(
            new ApiUnitDto()
            {
                Id = unit.Id,
                Name = "Degrees Celsius",
                Symbol = "°C"
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
        var response = await _httpClient.GetAsync($"units/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
