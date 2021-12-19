using System.Net.Http.Json;
using Fga.Net.Authentication;
using Fga.Net.Http;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Fga.Net.Authorization;
public class FgaAuthorizationClient : IDisposable
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
    public static FgaAuthorizationClient Create(FgaAuthenticationClient client, FgaClientConfiguration configuration)
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
    /// <summary>
    /// The check API will return whether the user has a certain relationship with an object in a certain store.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CheckTupleResponse?> CheckAsync(CheckTupleRequest request, CancellationToken ct = default)
    {
       var res= await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);
       res.EnsureSuccessStatusCode();
       return await res.Content.ReadFromJsonAsync<CheckTupleResponse>(cancellationToken: ct);
    }
    /// <summary>
    /// The POST read API will return the tuples for a certain store that matches a query filter specified in the body. Tuples and type definitions allow Auth0 FGA to determine whether a relationship exists between an object and an user.
    /// In the body: <example><code>Object is mandatory. An object can be a full object (e.g., type:object_id) or type only (e.g., type:).</code></example>
    /// <example><code>User is mandatory in the case the object is type only.</code></example>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<ReadTupleResponse?> ReadAsync(ReadTupleRequest request, CancellationToken ct = default)
    {
        var res = await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ReadTupleResponse>(cancellationToken: ct);
    }

    /// <summary>
    /// The POST write API will update the tuples for a certain store. Tuples and type definitions allow Auth0 FGA to determine whether a relationship exists between an object and an user.
    /// Path parameter store_id is required.In the body, writes adds new tuples while deletes remove existing tuples.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
