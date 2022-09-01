using OpenFga.Sdk.Api;
using OpenFga.Sdk.Model;

namespace Fga.Net.AspNetCore.Authorization;

/// <summary>
/// Temporary wrapper to allow for easier testing of middleware. Don't take a dependency on this.
/// </summary>
public class FgaCheckDecorator : IFgaCheckDecorator
{
    private readonly OpenFgaApi _auth0FgaApi;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auth0FgaApi"></param>
    public FgaCheckDecorator(OpenFgaApi auth0FgaApi)
    {
        _auth0FgaApi = auth0FgaApi;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public virtual Task<CheckResponse> Check(CheckRequest request, CancellationToken ct) => _auth0FgaApi.Check(request, ct);
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
    Task<CheckResponse> Check(CheckRequest request, CancellationToken ct);
}