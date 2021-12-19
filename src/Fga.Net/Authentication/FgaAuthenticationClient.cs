using System.Net.Http.Json;

namespace Fga.Net.Authentication;

public class FgaAuthenticationClient
{
    private readonly HttpClient _httpClient;

    public FgaAuthenticationClient(HttpClient client)
    {
        _httpClient = client;
    }

    public static FgaAuthenticationClient Create(HttpClient? client = null)
    {
        var httpClient = client ?? new HttpClient();
        httpClient.BaseAddress = new Uri(FgaConstants.AuthenticationUrl);
        return new FgaAuthenticationClient(httpClient);
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

        var res = await _httpClient.PostAsync("oauth/token", content);
        if (!res.IsSuccessStatusCode)
        {

        }

        return await res.Content.ReadFromJsonAsync<AccessTokenResponse>();
    }
}