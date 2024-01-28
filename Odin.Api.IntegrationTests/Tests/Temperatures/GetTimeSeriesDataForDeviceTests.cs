using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Odin.Api.Config;
using Odin.Api.Endpoints.ResponseSchemas;
using Odin.Api.IntegrationTests.Infrastructure;
using Odin.Api.Models;
using Xunit;

namespace Odin.Api.IntegrationTests.Tests.Temperatures;

[Collection(nameof(ApiCollection))]
public class GetTimeSeriesDataForDeviceTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.HttpClient;
    private readonly Func<Task> _resetDatabase = factory.ResetDatabaseAsync;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    [Fact]
    public async Task GetTimeSeriesData_TimeRangeIsWeek_ReturnsOkWithDailyAverages()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        var degreesCelsiusUnit = new Unit { Name = "Degrees Celsius", Symbol = "°C" };
        await factory.InsertAsync(degreesCelsiusUnit);

        var todayDate = DateTime.UtcNow.Date;

        var t1 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = todayDate.AddDays(-7),
            Value = 24.5,
            UnitId = degreesCelsiusUnit.Id
        };

        var t2 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = todayDate.AddDays(-7),
            Value = 25.5,
            UnitId = degreesCelsiusUnit.Id
        };

        var t3 = new Temperature()
        {
            DeviceId = device.Id,
            Timestamp = todayDate.AddDays(-1),
            Value = 26.5,
            UnitId = degreesCelsiusUnit.Id
        };

        await factory.InsertAsync(t1, t2, t3);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}/temperatures/time-series?timeRange=week");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseJson = await response.Content.ReadFromJsonAsync<IEnumerable<TimeSeriesDataPoint>>();
        var expectedDateFormat = DateTimeOffsetConstants.YearMonthDayFormat;
        responseJson.Should()
            .NotBeNull()
            .And.HaveCount(8)
            .And.SatisfyRespectively(
                first =>
                {
                    first.Timestamp.Should().Be(todayDate.AddDays(-7).ToString(expectedDateFormat));
                    first.Value.Should().Be((t1.Value + t2.Value) / 2);
                },
                second =>
                {
                    second.Timestamp.Should().Be(todayDate.AddDays(-6).ToString(expectedDateFormat));
                    second.Value.Should().BeNull();
                },
                third =>
                {
                    third.Timestamp.Should().Be(todayDate.AddDays(-5).ToString(expectedDateFormat));
                    third.Value.Should().BeNull();
                },
                fourth =>
                {
                    fourth.Timestamp.Should().Be(todayDate.AddDays(-4).ToString(expectedDateFormat));
                    fourth.Value.Should().BeNull();
                },
                fifth =>
                {
                    fifth.Timestamp.Should().Be(todayDate.AddDays(-3).ToString(expectedDateFormat));
                    fifth.Value.Should().BeNull();
                },
                sixth =>
                {
                    sixth.Timestamp.Should().Be(todayDate.AddDays(-2).ToString(expectedDateFormat));
                    sixth.Value.Should().BeNull();
                },
                seventh =>
                {
                    seventh.Timestamp.Should().Be(todayDate.AddDays(-1).ToString(expectedDateFormat));
                    seventh.Value.Should().Be(t3.Value);
                },
                eighth =>
                {
                    eighth.Timestamp.Should().Be(todayDate.ToString(expectedDateFormat));
                    eighth.Value.Should().BeNull();
                }
            );
    }

    [Fact]
    public async Task GetTimeSeriesData_InvalidRange_ReturnsBadRequest()
    {
        // Arrange
        var device = new Device { Name = "Arduino Uno R3 TMP36 Button Serial" };
        await factory.InsertAsync(device);

        // Act
        var response = await _httpClient.GetAsync($"devices/{device.Id}/temperatures/time-series?timeRange=invalid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
