namespace Fga.Net;

/// <summary>
/// Static FGA utilities
/// </summary>
public static class FgaUtilities
{
    /// <summary>
    /// Creates a new FGA Authorization <see cref="Uri"/> based on the provided environment
    /// </summary>
    /// <param name="environment">The environment, such as "us1"</param>
    /// <returns></returns>
    public static Uri GetAuthorizationUri(string environment) => new(string.Format(FgaConstants.AuthorizationUrl, environment));

}