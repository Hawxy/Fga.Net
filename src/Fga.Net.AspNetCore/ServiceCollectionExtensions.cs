using Fga.Net.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fga.Net.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Fga(this IServiceCollection collection, Action<FgaClientConfiguration> config)
    {
        collection.AddAuth0FgaAuthenticationClient();
        collection.AddAuth0FgaAuthorizationClient(config);
        collection.AddScoped<IAuthorizationHandler, SandcastleAuthorizationHandler>();
        return collection;
    }

    public static AuthorizationOptions AddFgaPolicy(this AuthorizationOptions options, string name = "Sandcastle")
    {
        options.AddPolicy(name, p => p.AddRequirements(new SandcastleRequirement()));
        return options;
    }
}
