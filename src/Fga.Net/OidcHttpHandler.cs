using System.Net.Http.Headers;
using OpenFga.Sdk.ApiClient;
using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

internal sealed class OidcHttpHandler : DelegatingHandler
{
    private const string Scheme = "Bearer";
    private readonly OAuth2Client _client;
    public OidcHttpHandler(OAuth2Client client)
    {
        _client = client;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _client.GetAccessTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, token);
        return await base.SendAsync(request, cancellationToken);
    }
}