#region License
/*
   Copyright 2021-2023 Hawxy

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

using Fga.Net.DependencyInjection.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Extensions for registering Fga features to a .NET environment.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and configures an <see cref="OpenFgaClient"/> and <see cref="OpenFgaApi"/> for the provided service collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configuration"></param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the <see cref="OpenFgaClient"/>.</returns>
    public static (IHttpClientBuilder openFgaApiBuilder, IHttpClientBuilder openFgaClientBuilder) AddOpenFgaClient(this IServiceCollection collection, Action<FgaClientConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var config = new FgaClientConfiguration();
        
        configuration.Invoke(config);

        if (config.Credentials?.Method is CredentialsMethod.ClientCredentials)
        {
            
        }

        collection.Configure<FgaClientConfiguration>(x =>
        {
            
        });

        return (collection.AddHttpClient<OpenFgaApi, InjectableFgaApi>(), collection.AddHttpClient<OpenFgaClient, InjectableFgaClient>());

    }


    public static void AddOpenFgaClient(this IServiceCollection collection, Action<FgaConfigurationRoot> config)
    {
        var apiClientBuilder = collection.AddHttpClient<OpenFgaApi, InjectableFgaApi>();
        var fgaClientBuilder = collection.AddHttpClient<OpenFgaClient, InjectableFgaClient>();
        
        var configRoot = new FgaConfigurationRoot();
        config.Invoke(configRoot);

        var connection = configRoot.GetConnectionConfiguration();

        collection.Configure<FgaClientConfiguration>(x=> 
            ConfigureOptions(x, configRoot, connection.Credentials?.Method is CredentialsMethod.ApiToken ? connection.Credentials : null));

        if (connection.Credentials?.Method is CredentialsMethod.ClientCredentials)
        {
            apiClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
            fgaClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
            
        }
        
    }
    
    
    private static void ConfigureOptions(FgaClientConfiguration x, FgaConfigurationRoot configRoot, Credentials? credentials)
    {
        x.StoreId = configRoot.StoreId;
        x.AuthorizationModelId = configRoot.AuthorizationModelId;
        if (configRoot.MaxRetry.HasValue)
            x.MaxRetry = configRoot.MaxRetry.Value;
        if (configRoot.MinWaitInMs.HasValue)
            x.MinWaitInMs = configRoot.MinWaitInMs.Value;

        x.Credentials = credentials;
    }

}

