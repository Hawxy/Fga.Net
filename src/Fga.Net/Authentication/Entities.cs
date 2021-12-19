using System.Text.Json.Serialization;

namespace Fga.Net.Authentication;

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