using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Model;

namespace Fga.Net.AspNetCore.Authorization;

/// <summary>
/// Temporary wrapper to allow for easier testing of middleware. Don't take a dependency on this.
/// </summary>
public class FgaCheckDecorator : IFgaCheckDecorator
{
    private readonly OpenFgaClient _auth0FgaApi;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auth0FgaApi"></param>
    public FgaCheckDecorator(OpenFgaClient auth0FgaApi)
    {
        _auth0FgaApi = auth0FgaApi;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public virtual Task<CheckResponse> Check(ClientCheckRequest request, CancellationToken ct) => _auth0FgaApi.Check(request, cancellationToken: ct);
}

/// <summary>
/// Temporary wrapper to allow for easier testing of middleware. Don't take a dependency on this.
/// </summary>
public interface IFgaCheckDecorator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<CheckResponse> Check(ClientCheckRequest request, CancellationToken ct);
}