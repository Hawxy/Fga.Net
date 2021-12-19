using Fga.Net.Authentication;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Fga.Net.Http;

internal class FgaTokenCache
{
    private readonly IAppCache _cache;
    private readonly FgaAuthenticationClient _client;
    private readonly FgaClientConfiguration _config;
    private const string CacheKey = "FgaToken";

    public FgaTokenCache(IAppCache cache, FgaAuthenticationClient client, FgaClientConfiguration config)
    {
        _cache = cache;
        _client = client;
        _config = config;
    }

    public FgaTokenCache(IAppCache cache, FgaAuthenticationClient client, IOptions<FgaClientConfiguration> config)
    {
        _cache = cache;
        _client = client;
        _config = config.Value;
    }

    public async Task<string> GetOrUpdateTokenAsync()
    {
        return await _cache.GetOrAddAsync(CacheKey, async entry =>
        {
            var res = await _client.FetchTokenAsync(new AccessTokenRequest
            {
                ClientId = _config.ClientId,
                ClientSecret = _config.ClientSecret,
                Environment = _config.Environment
            });

            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(res!.ExpiresIn).Subtract(TimeSpan.FromMinutes(15));

            return res.AccessToken;

        });

    }


}