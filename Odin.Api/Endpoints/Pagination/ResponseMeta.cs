namespace Odin.Api.Endpoints.Pagination;

public record ResponseMeta
{
    /// <summary>
    ///     The current page for this collection request.
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    ///     The maximum number of records per page.
    /// </summary>
    public required int Limit { get; init; }

    /// <summary>
    ///     Total number of the results which meets the search criteria regardless of the page and limit.
    /// </summary>
    public required int Total { get; init; }

    /// <summary>
    ///     The number of records in the current page.
    /// </summary>
    public required int Count { get; init; }
}
