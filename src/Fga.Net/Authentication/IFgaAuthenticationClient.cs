namespace Fga.Net.Authentication;

/// <summary>
/// The FGA Authentication Client
/// </summary>
public interface IFgaAuthenticationClient : IDisposable
{
    /// <summary>
    /// Requests an access token for the given FGA environment
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<AccessTokenResponse?> FetchTokenAsync(AccessTokenRequest request);
}