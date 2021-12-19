using System.Net.Http.Json;
using Fga.Net.Authentication;
using Fga.Net.Http;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Fga.Net.Authorization;
public class FgaAuthorizationClient : IDisposable
{
    // Rate limiting
    // error handling
    // etc
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

    public async Task<CheckResponse?> CheckAsync(CheckRequest request, CancellationToken ct = default)
    {
       var res= await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);

       return await res.Content.ReadFromJsonAsync<CheckResponse>(cancellationToken: ct);
    }

    public void Dispose()
    {
        if(_isInternalHttpClient)
            _client.Dispose();
    }
}
