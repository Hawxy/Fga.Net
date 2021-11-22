using System.Net.Http.Json;

namespace Sandcastle
{
    public class SandcastleAuthenticationClient
    {
        private readonly HttpClient _httpClient = new() {BaseAddress = new Uri(SandcastleConstants.Url)};
        public async Task<AccessTokenResponse?> FetchTokenAsync(AccessTokenRequest request)
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", request.GrantType },
                { "client_id", request.ClientId },
                { "client_secret", request.ClientSecret },
                { "audience", string.Format(SandcastleConstants.Audience, request.Environment) }
            };
            var content = new FormUrlEncodedContent(dict);

            var res = await _httpClient.PostAsync("/oauth/token", content);

            return await res.Content.ReadFromJsonAsync<AccessTokenResponse>();
        }
    }
}
