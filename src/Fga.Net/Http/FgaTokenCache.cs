#region License
/*
   Copyright 2021-2022 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using Fga.Net.Authentication;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Fga.Net.Http;

internal class FgaTokenCache : IFgaTokenCache
{
    private readonly IAppCache _cache;
    private readonly IFgaAuthenticationClient _client;
    private readonly FgaClientConfiguration _config;
    private const string CacheKey = "FgaToken";

    public FgaTokenCache(IAppCache cache, IFgaAuthenticationClient client, FgaClientConfiguration config)
    {
        _cache = cache;
        _client = client;
        _config = config;
    }

    public FgaTokenCache(IAppCache cache, IFgaAuthenticationClient client, IOptions<FgaClientConfiguration> config)
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
                Audience = string.Format(FgaConstants.AudienceFormat, _config.Environment)
            });

            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(res!.ExpiresIn).Subtract(TimeSpan.FromMinutes(15));

            return res.AccessToken;

        });

    }


}