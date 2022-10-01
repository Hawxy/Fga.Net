using System.Security.Claims;
using System.Security.Principal;
using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Net.AspNetCore;

/// <summary>
/// Configuration for FGAs Middleware
/// </summary>
public sealed class FgaAspNetCoreConfiguration
{
    /// <summary>
    /// A resolver that fetches the current user's identity based on the users <see cref="ClaimsPrincipal"/> during a network request. Used by all attributes derived from <see cref="FgaBaseObjectAttribute"/>.
    /// </summary>
    public Func<ClaimsPrincipal, string>? UserIdentityResolver { get; set; }
}