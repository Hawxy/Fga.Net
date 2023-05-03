using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection.Configuration;


/// <summary>
/// OpenFga API Scheme
/// </summary>
public enum Scheme {
    /// <summary>
    /// Sets the scheme to HTTP
    /// </summary>
    Http,
    /// <summary>
    /// Sets the scheme to HTTPS
    /// </summary>
    Https
}

/// <summary>
/// Configuration for OpenFga environments
/// </summary>
public sealed class OpenFgaConnectionBuilder
{
    private Scheme _apiScheme = Scheme.Https;
    private string? _apiHost;

    /// <summary>
    /// Sets the connection configuration for the host.
    /// </summary>
    /// <param name="apiScheme">API scheme, either HTTP or HTTPS</param>
    /// <param name="apiHost">API host, should be in be plain URI format</param>
    /// <returns></returns>
    public OpenFgaConnectionBuilder SetConnection(Scheme apiScheme, string apiHost)
    {
        _apiScheme = apiScheme;
        _apiHost = apiHost;
        return this;
    }

    private Credentials? _credentials;

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
        return new FgaConnectionConfiguration(_apiScheme, _apiHost, _credentials);
    }
}