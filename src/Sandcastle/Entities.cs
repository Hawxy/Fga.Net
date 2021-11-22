using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sandcastle;

public class AccessTokenRequest
{
    public string GrantType { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Audience { get; set; } = null!;

}

public record AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; init; } = null!;
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = null!;
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = null!;
}