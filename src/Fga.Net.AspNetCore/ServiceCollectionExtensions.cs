using Fga.Net.AspNetCore.Authorization;
using Fga.Net.Authentication;
using Fga.Net.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fga.Net.AspNetCore;

/// <summary>
/// Extensions for registering Fga.Net.AspNetCore features to an ASP.NET Core environment.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures an <see cref="SandcastleAuthorizationHandler"/> along with a <see cref="FgaAuthenticationClient"/> and <see cref="FgaAuthorizationClient"/>
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The delegate for the <see cref="FgaClientConfiguration"/> that will be used to configure the <see cref="FgaAuthorizationClient"/></param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAuth0Fga(this IServiceCollection collection, Action<FgaClientConfiguration> config)
    {
        collection.AddAuth0FgaAuthenticationClient();
        collection.AddAuth0FgaAuthorizationClient(config);
        collection.AddScoped<IAuthorizationHandler, SandcastleAuthorizationHandler>();
        return collection;
    }

    /// <summary>
    /// Adds an FGA Authorization requirement to the given policy.
    /// </summary>
    /// <param name="builder">The Authorization Policy Builder to configure</param>
    /// <returns>The authorization policy builder that is being configured</returns>
    public static AuthorizationPolicyBuilder AddFgaRequirement(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new SandcastleRequirement());
    }
}
