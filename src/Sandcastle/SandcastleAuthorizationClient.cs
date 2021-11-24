using System.Net.Http.Json;

namespace Sandcastle;
public class SandcastleAuthorizationClient
{
    // Rate limiting
    // error handling
    // etc
    private readonly HttpClient _client;
    private readonly SandcastleClientConfiguration _configuration;

    public SandcastleAuthorizationClient(SandcastleClientConfiguration configuration, HttpClient client)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<CheckResponse?> CheckAsync(CheckRequest request, CancellationToken ct = default)
    {
       var res= await _client.PostAsJsonAsync($"/{_configuration.StoreId}/check", request, ct);

       return await res.Content.ReadFromJsonAsync<CheckResponse>(cancellationToken: ct);
    }
  

}

public class SandcastleClientConfiguration
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Environment { get; set; } = null!;
    public string StoreId { get; set; } = null!;
}
