using System.Text.Json.Serialization;

namespace Fga.Net.Authorization;

public class CheckRequest
{
    [JsonPropertyName("tuple_key")]
    public TupleKey TupleKey { get; init; } = null!;

    [JsonPropertyName("authorization_model_id")]
    public string AuthorizationModelId { get; init; } = null!;
}

public class TupleKey
{
    [JsonPropertyName("user")]
    public string User { get; init; } = null!;
    [JsonPropertyName("relation")]
    public string Relation { get; init; } = null!;
    [JsonPropertyName("object")]
    public string Object { get; init; } = null!;

}

public record CheckResponse
{
    [JsonPropertyName("allowed")]
    public bool Allowed { get; init; }
}