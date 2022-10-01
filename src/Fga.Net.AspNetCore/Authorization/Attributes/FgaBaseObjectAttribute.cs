using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Base type for implementing attributes that use a configuration-driven user source.
/// </summary>
public abstract class FgaBaseObjectAttribute : FgaAttribute
{
    /// <inheritdoc />
    public override ValueTask<string> GetUser(HttpContext context)
    {
        var config = context.RequestServices.GetRequiredService<IOptions<FgaAspNetCoreConfiguration>>().Value;
        return ValueTask.FromResult(config.UserIdentityResolver!(context.User));
    }
}