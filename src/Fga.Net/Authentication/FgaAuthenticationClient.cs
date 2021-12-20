using System.Net.Http.Json;

namespace Fga.Net.Authentication;

/// <inheritdoc />
public class FgaAuthenticationClient : IFgaAuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly bool _isInternalHttpClient;


    public FgaAuthenticationClient(HttpClient client)
    {
        _httpClient = client;
    }

    private FgaAuthenticationClient(HttpClient client, bool isInternalHttp)
    {
        _httpClient = client;
        _isInternalHttpClient = isInternalHttp;
    }

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
            { "audience", string.Format(FgaConstants.Audience, request.Environment) }
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