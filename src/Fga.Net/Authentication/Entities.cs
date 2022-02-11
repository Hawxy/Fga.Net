#region License
/*
   Copyright 2021-2022 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using System.Text.Json.Serialization;

namespace Fga.Net.Authentication;

/// <summary>
/// A FGA access token request
/// </summary>
public class AccessTokenRequest
{

    /// <summary>
    /// The grant type used to fetch a token. Hardcoded to client credentials.
    /// </summary>
    public readonly string GrantType = "client_credentials";
    /// <summary>
    /// The Client Id used to authenticate with the FGA API.
    /// </summary>
    public string ClientId { get; set; } = null!;
    /// <summary>
    /// The Client Secret used to authenticate with the FGA API.
    /// </summary>
    public string ClientSecret { get; set; } = null!;
    /// <summary>
    /// The Audience this token is being requested for, usually in the format of a URL.
    /// </summary>
    public string Audience { get; set; } = null!;

}

/// <summary>
/// The access token response.
/// </summary>
public record AccessTokenResponse
{
    /// <summary>
    /// The access token used for FGA API access.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;
    /// <summary>
    /// The token's expiry in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public double ExpiresIn { get; init; }
    /// <summary>
    /// The returned scopes.
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = null!;
    /// <summary>
    /// The type of token returned.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = null!;
}