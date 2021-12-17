using Fga.Net.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fga.Net.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Fga(this IServiceCollection collection, Action<FgaClientConfiguration> config)
    {
        collection.AddLazyCache();
        collection.AddScoped<FgaTokenCache>();
        collection.Configure(config);
        collection.AddHttpClient<FgaAuthenticationClient>();
        collection.TryAddTransient<FgaTokenHandler>();
        collection.AddHttpClient<FgaAuthorizationClient, InjectableFgaAuthorizationClient>().AddHttpMessageHandler<FgaTokenHandler>();
        collection.AddScoped<IAuthorizationHandler, SandcastleAuthorizationHandler>();
        return collection;
    }

    public static AuthorizationOptions AddFgaPolicy(this AuthorizationOptions options, string name = "Sandcastle")
    {
        options.AddPolicy(name, p => p.AddRequirements(new SandcastleRequirement()));
        return options;
    }
}

public class InjectableFgaAuthorizationClient :FgaAuthorizationClient
{
    public InjectableFgaAuthorizationClient(HttpClient client, IOptions<FgaClientConfiguration> configuration) : base(client, configuration.Value)
    {
    }
}