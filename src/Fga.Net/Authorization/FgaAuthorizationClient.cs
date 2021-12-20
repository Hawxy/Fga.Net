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
using Fga.Net.Authentication;
using Fga.Net.Http;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Fga.Net.Authorization;

/// <inheritdoc />
public class FgaAuthorizationClient : IFgaAuthorizationClient
{
    //TODO Rate limiting
    //TODO Error handling
    
    private readonly HttpClient _client;
    private readonly FgaClientConfiguration _configuration;
    private readonly bool _isInternalHttpClient;


    /// <summary>
    /// Creates a new Authorization client. This constructor is designed for use with dependency injection.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    public FgaAuthorizationClient(HttpClient client, IOptions<FgaClientConfiguration> configuration)
    {
        _client = client;
        _configuration = configuration.Value;
    }

    private FgaAuthorizationClient(HttpClient client, FgaClientConfiguration configuration, bool isInternalHttpClient)
    {
        _client = client;
        _configuration = configuration;
        _isInternalHttpClient = isInternalHttpClient;
    }

    /// <summary>
    /// Creates a standalone instance of the authorization client with an internally managed HttpClient and token caching middleware enabled by default.
    /// This client should be long-lived and disposed after use.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static FgaAuthorizationClient Create(IFgaAuthenticationClient client, FgaClientConfiguration configuration)
    {
        var cache = new CachingService();
        var tokenCache = new FgaTokenCache(cache, client, configuration);

        var httpClient = new HttpClient(new FgaTokenHandler(tokenCache));
        httpClient.BaseAddress = FgaUtilities.GetAuthorizationUri(configuration.Environment);

        return new FgaAuthorizationClient(httpClient, configuration, true);
    }

    /// <summary>
    /// Creates a standalone instance of the authorization client with your own <see cref="HttpClient"/>. No defaults are set.
    /// The lifetime of the HttpClient should be managed yourself.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <returns></returns>
    public static FgaAuthorizationClient Create(FgaClientConfiguration configuration, HttpClient httpClient)
    {
        return new FgaAuthorizationClient(httpClient, configuration, false);
    }

    /// <inheritdoc />
    public async Task<CheckTupleResponse?> CheckAsync(CheckTupleRequest request, CancellationToken ct = default)
    {
       var res= await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);
       res.EnsureSuccessStatusCode();
       return await res.Content.ReadFromJsonAsync<CheckTupleResponse>(cancellationToken: ct);
    }
    /// <inheritdoc />
    public async Task<ReadTupleResponse?> ReadAsync(ReadTupleRequest request, CancellationToken ct = default)
    {
        var res = await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ReadTupleResponse>(cancellationToken: ct);
    }

    /// <inheritdoc />
    public async Task WriteAsync(WriteTupleRequest request, CancellationToken ct = default)
    {
        var res = await _client.PostAsJsonAsync($"/{_configuration.StoreId}/write", request, ct);
        res.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if(_isInternalHttpClient)
            _client.Dispose();
    }
}
