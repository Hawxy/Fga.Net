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
using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Extensions for <see cref="FgaClientConfiguration"/>
/// </summary>
public static class FgaClientConfigurationExtensions
{

    /// <summary>
    /// Configures the client with connection defaults when using Auth0 FGA
    /// </summary>
    /// <param name="config">An instance of the configuration</param>
    /// <param name="clientId">The Auth0 FGA client ID</param>
    /// <param name="clientSecret">The Auth0 FGA client secret</param>
    public static void WithAuth0FgaDefaults(this FgaClientConfiguration config, string clientId, string clientSecret)
    {
        //TODO make environment configurable
        config.ApiHost = "api.us1.fga.dev";
        config.Credentials = new Credentials()
        {
            Method = CredentialsMethod.ClientCredentials,
            Config = new CredentialsConfig()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                ApiTokenIssuer = "fga.us.auth0.com",
                ApiAudience = "https://api.us1.fga.dev/"
            }
        };
    }

}