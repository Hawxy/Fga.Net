using Auth0.Fga.Api;
using Auth0.Fga.Configuration;
using Microsoft.Extensions.Options;

namespace Fga.Net
{
    internal class InjectableAuth0FgaApi : Auth0FgaApi
    {
        public InjectableAuth0FgaApi(IOptions<FgaClientConfiguration> configuration, HttpClient httpClient) : base(configuration.Value, httpClient)
        {
        }
    }
}
