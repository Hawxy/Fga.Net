using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection.Configuration;

/// <summary>
/// Available environments for Auth0 FGA
/// </summary>
public enum Auth0Environment
{
    /// <summary>
    /// US Environment - `fga.us.auth0.com`
    /// </summary>
    Us
}

internal record Auth0FgaEnvironment(Scheme Scheme, string ApiHost, string ApiTokenIssuer, string ApiAudience);


/// <summary>
/// Configuration for Auth0 FGA environments
/// </summary>
public class Auth0FgaConnectionBuilder
{
    private readonly IReadOnlyDictionary<Auth0Environment, Auth0FgaEnvironment> _fgaEnvironments =
        new Dictionary<Auth0Environment, Auth0FgaEnvironment>()
        {
            {
                Auth0Environment.Us,
                new Auth0FgaEnvironment(Scheme.Https, "api.us1.fga.dev", "fga.us.auth0.com", "https://api.us1.fga.dev/")
            }
        };

    private readonly Auth0Environment _environment = Auth0Environment.Us;

    private string _clientId;
    private string _clientSecret;
 
    public void WithAuthentication(string clientId, string clientSecret)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(clientSecret);
        
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    internal FgaConnectionConfiguration Build()
    {
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

        return new FgaConnectionConfiguration(environment.Scheme, environment.ApiHost, credentials);
    }
}