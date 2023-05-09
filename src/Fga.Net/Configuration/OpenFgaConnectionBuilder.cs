using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection.Configuration;

/// <summary>
/// Configuration for OpenFga environments
/// </summary>
public sealed class OpenFgaConnectionBuilder
{
    private string _apiScheme = HttpScheme.Https;
    private string? _apiHost;

    /// <summary>
    /// Sets the connection configuration for the host.
    /// </summary>
    /// <param name="apiScheme">API scheme, either http or https. <see cref="HttpScheme"/></param>
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
        if (_apiScheme != HttpScheme.Https && _apiScheme != HttpScheme.Http)
            throw new InvalidOperationException("API Scheme must be http or https");
        
        return new FgaConnectionConfiguration(_apiScheme, _apiHost, _credentials);
    }
}