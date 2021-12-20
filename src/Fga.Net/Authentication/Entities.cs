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