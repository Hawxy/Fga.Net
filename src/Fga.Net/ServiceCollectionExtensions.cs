using Fga.Net.Authentication;
using Fga.Net.Authorization;
using Fga.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fga.Net;

public static  class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddAuth0FgaAuthenticationClient(this IServiceCollection collection)
    {
        return collection.AddHttpClient<FgaAuthenticationClient>(x=> x.BaseAddress = new Uri(FgaConstants.AuthenticationUrl));
    }

    public static IHttpClientBuilder AddAuth0FgaAuthorizationClient(this IServiceCollection collection, Action<FgaClientConfiguration> configuration)
    {
        collection.AddLazyCache();
        collection.AddScoped<FgaTokenCache>();
        collection.Configure(configuration);
        collection.TryAddTransient<FgaTokenHandler>();
        return collection.AddHttpClient<FgaAuthorizationClient>((services, client) =>
        {
            var config = services.GetRequiredService<IOptions<FgaClientConfiguration>>();
            client.BaseAddress = FgaUtilities.GetAuthorizationUri(config.Value.Environment);
        }).AddHttpMessageHandler<FgaTokenHandler>();
    }
}