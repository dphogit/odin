using System.Text.Json.Serialization;

namespace Odin.Api.Endpoints.Pagination;

public record PaginatedResponseSchema<T>
{
    public required IEnumerable<T> Data { get; init; }

    [JsonPropertyName("_meta")]
    public required ResponseMeta Meta { get; init; }
}
