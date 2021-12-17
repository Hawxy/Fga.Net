using System.Text.Json.Serialization;

namespace Fga.Net;

public class AccessTokenRequest
{
    public readonly string GrantType = "client_credentials";
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Environment { get; set; } = null!;

}

public record AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;
    [JsonPropertyName("expires_in")]
    public double ExpiresIn { get; init; }
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = null!;
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = null!;
}

public class CheckRequest
{
    [JsonPropertyName("tuple_key")]
    public TupleKey TupleKey { get; init; } = null!;

    [JsonPropertyName("authorization_model_id")]
    public string AuthorizationModelId { get; init; } = null!;
}

public class TupleKey
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = null!;
    [JsonPropertyName("relation")]
    public string Relation { get; init; } = null!;
    [JsonPropertyName("user")]
    public string User { get; init; } = null!;
}

public record CheckResponse
{
    [JsonPropertyName("allowed")]
    public bool Allowed { get; init; }
}