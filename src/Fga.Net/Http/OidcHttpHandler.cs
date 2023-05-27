using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.ApiClient;

namespace Fga.Net.DependencyInjection.Http;

internal sealed class OidcHttpHandler : DelegatingHandler
{
    private const string Scheme = "Bearer";
    private readonly OAuth2Client _client;
    public OidcHttpHandler(OAuth2Client client, IOptions<FgaBuiltConfiguration> )
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