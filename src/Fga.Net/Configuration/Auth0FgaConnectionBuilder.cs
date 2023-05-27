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

internal sealed record Auth0FgaEnvironment(string Scheme, string ApiHost, string ApiTokenIssuer, string ApiAudience);


/// <summary>
/// Configuration for Auth0 FGA environments
/// </summary>
public sealed class Auth0FgaConnectionBuilder
{
    private readonly IReadOnlyDictionary<Auth0Environment, Auth0FgaEnvironment> _fgaEnvironments =
        new Dictionary<Auth0Environment, Auth0FgaEnvironment>()
        {
            {
                Auth0Environment.Us,
                new Auth0FgaEnvironment(Uri.UriSchemeHttps, "api.us1.fga.dev", "fga.us.auth0.com", "https://api.us1.fga.dev/")
            }
        };

    private readonly Auth0Environment _environment = Auth0Environment.Us;

    private string _clientId = null!;
    private string _clientSecret = null!;

    
    /// <summary>
    /// Configure authentication for Auth0 FGA
    /// </summary>
    /// <param name="clientId">Client Id from your  Auth0 FGA Account</param>
    /// <param name="clientSecret">Client Secret from your Auth0 FGA Account</param>
    public void WithAuthentication(string clientId, string clientSecret)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(clientSecret);
        
        _clientId = clientId;
        _clientSecret = clientSecret;
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

        return new FgaConnectionConfiguration(environment.Scheme, environment.ApiHost, credentials);
    }
}