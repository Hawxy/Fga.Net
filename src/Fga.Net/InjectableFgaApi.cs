using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;

namespace Fga.Net.DependencyInjection;

internal sealed class InjectableFgaApi : OpenFgaApi
{
    public InjectableFgaApi(IOptions<FgaClientConfiguration> configuration, HttpClient httpClient) : base(configuration.Value, httpClient)
    {
    }
}