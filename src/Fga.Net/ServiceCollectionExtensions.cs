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

using Microsoft.Extensions.DependencyInjection;
using OpenFga.Sdk.Api;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Extensions for registering Fga features to a .NET environment.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and configures an <see cref="OpenFgaApi"/> for the provided service collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configuration"></param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the <see cref="FgaClientConfiguration"/>.</returns>
    public static IHttpClientBuilder AddOpenFgaClient(this IServiceCollection collection, Action<FgaClientConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        collection.Configure(configuration);
        return collection.AddHttpClient<OpenFgaApi, InjectableFgaApi>();
    }
}

