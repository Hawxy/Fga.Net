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

using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection.Configuration;

/// <summary>
/// Configuration for OpenFga environments
/// </summary>
public sealed class OpenFgaConnectionBuilder
{
    private string _apiScheme = Uri.UriSchemeHttps;
    private string? _apiHost;

    /// <summary>
    /// Sets the connection configuration for the host.
    /// </summary>
    /// <param name="apiScheme">API scheme, either http or https.</param>
    /// <param name="apiHost">API host, should be in be plain URI format</param>
    /// <returns></returns>
    public OpenFgaConnectionBuilder SetConnection(string apiScheme, string apiHost)
    {
        _apiScheme = apiScheme;
        _apiHost = apiHost;
        return this;
    }

    private Credentials? _credentials;

    /// <summary>
    /// Configures the OpenFGA client with API Key authentication.
    /// Your FGA instance must be configured to support Key Authentication. See https://openfga.dev/docs/getting-started/setup-openfga/docker#pre-shared-key-authentication
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    public void WithApiKeyAuthentication(string apiKey)
    {
        _credentials = new Credentials()
        {
            Method = CredentialsMethod.ApiToken,
            Config = new CredentialsConfig()
            {
                ApiToken = apiKey
            }
        };
    }

    /// <summary>
    /// Configures the OpenFGA client with OIDC authentication (aka Client Credentials flow).
    /// Your FGA instance must be configured to support OIDC Authentication. See https://openfga.dev/docs/getting-started/setup-openfga/docker#oidc
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="clientSecret">Client Secret</param>
    /// <param name="issuer">The token issuer</param>
    /// <param name="audience">The audience of your FGA instance</param>
    public void WithOidcAuthentication(string clientId, string clientSecret, string issuer, string audience)
    {
        _credentials = new Credentials()
        {
            Method = CredentialsMethod.ClientCredentials,
            Config = new CredentialsConfig()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                ApiTokenIssuer = issuer,
                ApiAudience = audience
            }
        };
    }

    internal FgaConnectionConfiguration Build()
    {
        if (string.IsNullOrEmpty(_apiHost))
            throw new InvalidOperationException("API Host cannot be null or empty");
        if (_apiScheme != Uri.UriSchemeHttps && _apiScheme != Uri.UriSchemeHttp)
            throw new InvalidOperationException("API Scheme must be http or https");
        if (_credentials?.Method == CredentialsMethod.ApiToken && string.IsNullOrEmpty(_credentials.Config?.ApiToken))
            throw new InvalidOperationException("API Key cannot be empty");
        if(_credentials?.Method == CredentialsMethod.ClientCredentials 
           && (string.IsNullOrEmpty(_credentials.Config?.ClientId) 
               || string.IsNullOrEmpty(_credentials.Config?.ClientSecret) 
               || string.IsNullOrEmpty(_credentials.Config?.ApiTokenIssuer) 
               || string.IsNullOrEmpty(_credentials.Config?.ApiAudience))) 
            throw new InvalidOperationException("Clients credential configuration cannot be contain missing values.");
        
        return new FgaConnectionConfiguration(_apiScheme, _apiHost, _credentials);
    }
}