using System.Net.Http.Json;

namespace Fga.Net;
public class FgaAuthorizationClient
{
    // Rate limiting
    // error handling
    // etc
    private readonly HttpClient _client;
    private readonly FgaClientConfiguration _configuration;

    public FgaAuthorizationClient(HttpClient client, FgaClientConfiguration configuration)
    {
        client.BaseAddress = new Uri(string.Format(FgaConstants.AuthorizationUrl, configuration.Environment));
        _client = client;

        _configuration = configuration;
    }

    public async Task<CheckResponse?> CheckAsync(CheckRequest request, CancellationToken ct = default)
    {
       var res= await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);

       return await res.Content.ReadFromJsonAsync<CheckResponse>(cancellationToken: ct);
    }
}
