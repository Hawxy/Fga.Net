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

using System.Net.Http.Headers;

namespace Fga.Net.Http;

/// <summary>
/// A <see cref="DelegatingHandler"/> that adds a authentication header with a Auth0-generated JWT token for the Fga request
/// </summary>
internal class FgaTokenHandler : DelegatingHandler
{
    private const string Scheme = "Bearer";
    private readonly IFgaTokenCache _cache;
    /// <summary>
    /// Constructs a new instance of the <see cref="FgaTokenHandler"/>
    /// </summary>
    /// <param name="cache">An instance of an <see cref="FgaTokenCache"/>.</param>
    public FgaTokenHandler(IFgaTokenCache cache)
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