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
using Fga.Net.Authorization;
using Fga.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fga.Net;

/// <summary>
/// Extensions for registering Fga.Net features to an .NET environment.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="FgaAuthenticationClient"/> for the provided service collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the <see cref="FgaAuthenticationClient"/>.</returns>
    public static IHttpClientBuilder AddAuth0FgaAuthenticationClient(this IServiceCollection collection)
    {
        return collection.AddHttpClient<IFgaAuthenticationClient, FgaAuthenticationClient>(x=> x.BaseAddress = new Uri(FgaConstants.AuthenticationUrl));
    }

    /// <summary>
    /// Registers and configures an <see cref="FgaAuthorizationClient"/> for the provided service collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configuration"></param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the <see cref="FgaClientConfiguration"/>.</returns>
    public static IHttpClientBuilder AddAuth0FgaAuthorizationClient(this IServiceCollection collection, Action<FgaClientConfiguration> configuration)
    {
        collection.AddLazyCache();
        collection.AddScoped<IFgaTokenCache, FgaTokenCache>();
        collection.Configure(configuration);
        collection.TryAddTransient<FgaTokenHandler>();
        return collection.AddHttpClient<IFgaAuthorizationClient, FgaAuthorizationClient>((services, client) =>
        {
            var config = services.GetRequiredService<IOptions<FgaClientConfiguration>>();
            client.BaseAddress = FgaUtilities.GetAuthorizationUri(config.Value.Environment);
        }).AddHttpMessageHandler<FgaTokenHandler>();
    }
}