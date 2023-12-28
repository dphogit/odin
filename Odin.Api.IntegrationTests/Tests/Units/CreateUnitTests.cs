using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Shared.ApiDtos.Units;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Units;

[Collection(nameof(ApiCollection))]
public class CreateUnitTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Create_ValidBody_InsertsUnitIntoDbAndReturnsCreatedWithHeaderLocation()
    {
        // Arrange
        ApiCreateUnitDto createUnitDTO = new()
        {
            Name = "Unit 1",
            Symbol = "Symbol 1"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("units", createUnitDTO);
        var unitDTO = await response.Content.ReadFromJsonAsync<ApiUnitDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        unitDTO.Should().BeOfType<ApiUnitDto>().Which.Id.Should().BeOfType(typeof(int));
        unitDTO.Should().BeEquivalentTo(createUnitDTO);
        response.Headers.Location.Should().BeOfType<Uri>()
            .Which.AbsolutePath.Should().Be($"/units/{unitDTO!.Id}");
    }
}
