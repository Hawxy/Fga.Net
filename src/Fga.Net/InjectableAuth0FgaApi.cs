using Auth0.Fga.Api;
using Microsoft.Extensions.Options;

namespace Fga.Net.DependencyInjection
{
    internal class InjectableAuth0FgaApi : Auth0FgaApi
    {
        public InjectableAuth0FgaApi(IOptions<FgaClientConfiguration> configuration, HttpClient httpClient) : base(configuration.Value, httpClient)
        {
        }
    }
}
