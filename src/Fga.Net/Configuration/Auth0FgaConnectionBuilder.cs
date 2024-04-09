#region License
/*
   Copyright 2021-2024 Hawxy (JT)

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
/// Available environments for Auth0 FGA
/// </summary>
public enum FgaEnvironment
{
    /// <summary>
    /// US Environment - `api.us1.fga.dev`
    /// </summary>
    US,
    /// <summary>
    /// AU Environment - `api.au1.fga.dev`
    /// </summary>
    AU,
    /// <summary>
    /// EU Environment - `api.eu1.fga.dev`
    /// </summary>
    EU
}

internal sealed record Auth0FgaEnvironment(string ApiHost, string ApiTokenIssuer, string ApiAudience);


/// <summary>
/// Configuration for Auth0 FGA environments
/// </summary>
public sealed class Auth0FgaConnectionBuilder
{
    private const string FgaIssuer = "fga.us.auth0.com";
    
    private readonly IReadOnlyDictionary<FgaEnvironment, Auth0FgaEnvironment> _fgaEnvironments =
        new Dictionary<FgaEnvironment, Auth0FgaEnvironment>()
        {
            {
                FgaEnvironment.US,
                new Auth0FgaEnvironment("https://api.us1.fga.dev", FgaIssuer, "https://api.us1.fga.dev/")
            },
            {
                FgaEnvironment.EU,
                new Auth0FgaEnvironment("https://api.eu1.fga.dev", FgaIssuer, "https://api.eu1.fga.dev/")
            },
            {
                FgaEnvironment.AU,
                new Auth0FgaEnvironment("https://api.au1.fga.dev", FgaIssuer, "https://api.au1.fga.dev/")
            }
        };

    private FgaEnvironment _environment = FgaEnvironment.US;

    private string _clientId = null!;
    private string _clientSecret = null!;

    /// <summary>
    /// Set the region/environment that your Auth0 FGA store lives in. Defaults to <see cref="FgaEnvironment.US"/> if not set.
    /// </summary>
    /// <param name="environment">An Auth0 FGA region</param>
    public Auth0FgaConnectionBuilder SetEnvironment(FgaEnvironment environment)
    {
        _environment = environment;
        return this;
    }
    
    /// <summary>
    /// Configure authentication for Auth0 FGA
    /// </summary>
    /// <param name="clientId">Client Id from your  Auth0 FGA Account</param>
    /// <param name="clientSecret">Client Secret from your Auth0 FGA Account</param>
    public Auth0FgaConnectionBuilder WithAuthentication(string clientId, string clientSecret)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(clientSecret);
        
        _clientId = clientId;
        _clientSecret = clientSecret;
        return this;
    }

    internal FgaConnectionConfiguration Build()
    {
        if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            throw new InvalidOperationException("Auth0 FGA ClientId and ClientSecret must be set to non-empty values");
        
        var environment = _fgaEnvironments[_environment];

        var credentials = new Credentials()
        {
            Method = CredentialsMethod.ClientCredentials,
            Config = new CredentialsConfig()
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                ApiTokenIssuer = environment.ApiTokenIssuer,
                ApiAudience = environment.ApiAudience
            }
        };

        return new FgaConnectionConfiguration(environment.ApiHost, credentials);
    }
}