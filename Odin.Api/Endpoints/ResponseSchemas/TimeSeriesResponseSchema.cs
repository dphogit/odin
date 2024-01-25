namespace Odin.Api.Endpoints.ResponseSchemas;

public class TimeSeriesDataPoint
{
    public required string Timestamp { get; set; }

    public double? Value { get; set; }
}

public enum TimeRange
{
    Year,
    Month,
    Week,
}
