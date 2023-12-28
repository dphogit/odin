using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Odin.Api.Database;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Odin.Shared.ApiDtos.Units;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Units;

[Collection(nameof(ApiCollection))]
public class GetAllUnitTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    [Fact]
    public async Task Get_UnitsExist_ReturnsUnits()
    {
        // Arrange
        var unit1 = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        var unit2 = new Unit { Name = "Degrees Fahrenheit", Symbol = "°F" };
        await factory.InsertAsync(unit1, unit2);

        using var scope = factory.ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var id1 = dbContext.Units.Single(d => d.Name == "Degrees Celsius").Id;
        var id2 = dbContext.Units.Single(d => d.Name == "Degrees Fahrenheit").Id;

        // Act
        var response = await _httpClient.GetAsync("units");
        var unitDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ApiUnitDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        unitDtos.Should().NotBeNull().And.Satisfy(
            u => u.Id == id1 && u.Name == "Degrees Celsius" && u.Symbol == "°C",
            u => u.Id == id2 && u.Name == "Degrees Fahrenheit" && u.Symbol == "°F"
        );
    }
}
