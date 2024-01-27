using Odin.Api.Endpoints.Pagination;

namespace Odin.Api.Services;

public class GetTemperatureOptions
{
    public bool WithDevice { get; set; } = false;

    public int Page { get; set; } = 1;

    public int Limit { get; set; } = PaginationConstants.DefaultPaginationLimit;

    public TimestampSortOptions TimestampSort { get; set; } = TimestampSortOptions.Descending;

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }
}
