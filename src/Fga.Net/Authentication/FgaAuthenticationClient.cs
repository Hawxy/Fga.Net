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

using System.Net.Http.Json;

namespace Fga.Net.Authentication;

/// <inheritdoc />
public class FgaAuthenticationClient : IFgaAuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly bool _isInternalHttpClient;


    /// <summary>
    /// Primary constructor used internally for <see cref="IHttpClientFactory"/> integration.
    /// See <see cref="Create"/> if you want a standalone client.
    /// </summary>
    /// <param name="client"></param>
    public FgaAuthenticationClient(HttpClient client)
    {
        _httpClient = client;
    }

    private FgaAuthenticationClient(HttpClient client, bool isInternalHttp)
    {
        _httpClient = client;
        _isInternalHttpClient = isInternalHttp;
    }

    /// <summary>
    /// Creates a new FGA authentication client. Sets the authentication URL automatically.
    /// Optionally provide a HttpClient if you wish to maintain the lifetime yourself.
    /// </summary>
    /// <param name="client">A optional HttpClient, if none is provided one will be created.</param>
    /// <returns><see cref="FgaAuthenticationClient"/></returns>
    public static FgaAuthenticationClient Create(HttpClient? client = null)
    {
        var httpClient = client ?? new HttpClient();
        httpClient.BaseAddress = new Uri(FgaConstants.AuthenticationUrl);
        return new FgaAuthenticationClient(httpClient, client is null);
    }
    /// <inheritdoc />
    public async Task<AccessTokenResponse?> FetchTokenAsync(AccessTokenRequest request)
    {
        var dict = new Dictionary<string, string>
        {
            { "grant_type", request.GrantType },
            { "client_id", request.ClientId },
            { "client_secret", request.ClientSecret },
            { "audience", request.Audience }
        };
        var content = new FormUrlEncodedContent(dict);

        var res = await _httpClient.PostAsync("oauth/token", content);

        return await res.Content.ReadFromJsonAsync<AccessTokenResponse>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if(_isInternalHttpClient)
            _httpClient.Dispose();
    }
}