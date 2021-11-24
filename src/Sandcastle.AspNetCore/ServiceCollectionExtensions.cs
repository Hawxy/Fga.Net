using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sandcastle.AspNetCore.Authorization;

namespace Sandcastle.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Sandcastle(this IServiceCollection collection)
    {
        collection.AddHttpClient<SandcastleAuthenticationClient>();
        collection.AddHttpClient<SandcastleAuthorizationHandler>();
        collection.AddScoped<IAuthorizationHandler, SandcastleAuthorizationHandler>();
        return collection;
    }

    public static AuthorizationOptions AddSandcastlePolicy(this AuthorizationOptions options, string name = "Sandcastle")
    {
        options.AddPolicy(name, p => p.AddRequirements(new SandcastleRequirement()));
        return options;
    }
}