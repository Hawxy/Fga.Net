using System.Net.Http.Json;

namespace Fga.Net;

public class FgaAuthenticationClient
{
    private readonly HttpClient _httpClient;

    public FgaAuthenticationClient(HttpClient? client)
    {
        _httpClient = client ?? new HttpClient { BaseAddress = new Uri(FgaConstants.AuthenticationUrl) };
    }
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

        var res = await _httpClient.PostAsync("/oauth/token", content);

        return await res.Content.ReadFromJsonAsync<AccessTokenResponse>();
    }
}