using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="FgaClientConfiguration"/>
/// </summary>
public static class FgaClientConfigurationExtensions
{

    /// <summary>
    /// Configures the client with defaults when using Auth0 FGA
    /// </summary>
    /// <param name="config">An instance of the configuration</param>
    /// <param name="clientId">The client ID</param>
    /// <param name="clientSecret">The client secret</param>
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