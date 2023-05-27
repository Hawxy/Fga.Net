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
using Fga.Net.DependencyInjection.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.ApiClient;
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
    /// <param name="fgaConfiguration"></param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the <see cref="OpenFgaClient"/>.</returns>
    public static (IHttpClientBuilder openFgaApiBuilder, IHttpClientBuilder openFgaClientBuilder) AddOpenFgaClient(this IServiceCollection collection, Action<FgaConfigurationBuilder> fgaConfiguration)
    {
        var apiClientBuilder = collection.AddHttpClient<OpenFgaApi, InjectableFgaApi>();
        var fgaClientBuilder = collection.AddHttpClient<OpenFgaClient, InjectableFgaClient>();
        
        var configRoot = new FgaConfigurationBuilder();
        fgaConfiguration.Invoke(configRoot);

        var config = configRoot.Build();

        collection.Configure<FgaClientConfiguration>(x=> 
            x.ConfigureFgaOptions(config));

        // Custom handling of the client credentials flow as we need the OAuth2 client to store handle token lifetimes outside of the client itself
        if (config.Connection.Credentials?.Method is CredentialsMethod.ClientCredentials)
        {
            collection.AddHttpClient<BaseClient, InjectableBaseClient>()
                .ConfigurePrimaryHttpMessageHandler(() => 
                    new SocketsHttpHandler()
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                })
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            
            collection.AddSingleton<OAuth2Client>(provider => new OAuth2Client(config.Connection.Credentials, provider.GetRequiredService<BaseClient>()));

            collection.AddTransient<OidcHttpHandler>();

            apiClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
            fgaClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
        }

        return (apiClientBuilder, fgaClientBuilder);
    }

    public static void PostConfigureFgaClient(this IServiceCollection collection, Action<FgaConfigurationBuilder> fgaConfiguration)
    {
        var configRoot = new FgaConfigurationBuilder();
        fgaConfiguration.Invoke(configRoot);

        var config = configRoot.Build();

        collection.PostConfigure<FgaClientConfiguration>(x=> 
            x.ConfigureFgaOptions(config));
        
        
    }


    private static (IHttpClientBuilder openFgaApiBuilder, IHttpClientBuilder openFgaClientBuilder) ConfigureFgaClientInternal(this IServiceCollection collection,
        FgaBuiltConfiguration config)
    {
        var apiClientBuilder = collection.AddHttpClient<OpenFgaApi, InjectableFgaApi>();
        var fgaClientBuilder = collection.AddHttpClient<OpenFgaClient, InjectableFgaClient>();
        
        // Custom handling of the client credentials flow as we need the OAuth2 client to store handle token lifetimes outside of the client itself
        if (config.Connection.Credentials?.Method is CredentialsMethod.ClientCredentials)
        {
            collection.AddHttpClient<BaseClient, InjectableBaseClient>()
                .ConfigurePrimaryHttpMessageHandler(() => 
                    new SocketsHttpHandler()
                    {
                        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                    })
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            
            collection.AddSingleton<OAuth2Client>(provider => new OAuth2Client(config.Connection.Credentials, provider.GetRequiredService<BaseClient>()));

            collection.AddTransient<OidcHttpHandler>();

            apiClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
            fgaClientBuilder.AddHttpMessageHandler<OidcHttpHandler>();
        }

        return (apiClientBuilder, fgaClientBuilder);
    }
    
    
    private static void ConfigureFgaOptions(this FgaClientConfiguration x, FgaBuiltConfiguration config)
    {
        x.ApiScheme = config.Connection.ApiScheme;
        x.ApiHost = config.Connection.ApiHost;
        
        x.StoreId = config.StoreId;
        x.AuthorizationModelId = config.AuthorizationModelId;
        if (config.MaxRetry.HasValue)
            x.MaxRetry = config.MaxRetry.Value;
        if (config.MinWaitInMs.HasValue)
            x.MinWaitInMs = config.MinWaitInMs.Value;

        x.Credentials = config.Connection.Credentials?.Method is CredentialsMethod.ApiToken ? config.Connection.Credentials : null;
    }

}

