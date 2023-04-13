using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;

namespace Fga.Net.DependencyInjection;

internal sealed class InjectableFgaApi : OpenFgaApi
{
    public InjectableFgaApi(IOptions<FgaClientConfiguration> configuration, HttpClient httpClient) : base(configuration.Value, httpClient)
    {
    }
}

internal sealed class InjectableFgaClient : OpenFgaClient
{
    public InjectableFgaClient(IOptions<FgaClientConfiguration> configuration, HttpClient? httpClient = null) : base(configuration.Value, httpClient)
    {
    }
}