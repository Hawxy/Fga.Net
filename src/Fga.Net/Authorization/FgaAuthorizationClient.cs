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
using Fga.Net.Http;
using LazyCache;

namespace Fga.Net.Authorization;

/// <inheritdoc />
public partial class FgaAuthorizationClient
{
    /// <summary>
    /// Creates a standalone instance of the authorization client with an internally managed HttpClient and token caching middleware enabled by default.
    /// This client should be long-lived and disposed after use.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static FgaAuthorizationClient Create(IFgaAuthenticationClient client, FgaClientConfiguration configuration)
    {
        var cache = new CachingService();
        var tokenCache = new FgaTokenCache(cache, client, configuration);

        var httpClient = new HttpClient(new FgaTokenHandler(tokenCache))
        {
            BaseAddress = FgaUtilities.GetAuthorizationUri(configuration.Environment)
        };

        return new FgaAuthorizationClient(httpClient);
    }

    /// <summary>
    /// Creates a standalone instance of the authorization client with your own <see cref="HttpClient"/>. No defaults are set.
    /// The lifetime of the HttpClient should be managed yourself.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <returns></returns>
    public static FgaAuthorizationClient Create(HttpClient httpClient)
    {
        return new FgaAuthorizationClient(httpClient);
    }
}
