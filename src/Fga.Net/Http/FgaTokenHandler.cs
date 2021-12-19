using System.Net.Http.Headers;

namespace Fga.Net.Http;

/// <summary>
/// A <see cref="DelegatingHandler"/> that adds a authentication header with a Auth0-generated JWT token for the Fga request
/// </summary>
public class FgaTokenHandler : DelegatingHandler
{
    private const string Scheme = "Bearer";
    private readonly FgaTokenCache _cache;
    /// <summary>
    /// Constructs a new instance of the <see cref="FgaTokenHandler"/>
    /// </summary>
    /// <param name="cache">An instance of an <see cref="FgaTokenCache"/>.</param>
    public FgaTokenHandler(FgaTokenCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc cref="DelegatingHandler"/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, await _cache.GetOrUpdateTokenAsync());
        return await base.SendAsync(request, cancellationToken);
    }
}