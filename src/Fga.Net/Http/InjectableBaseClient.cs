using Microsoft.Extensions.Options;
using OpenFga.Sdk.ApiClient;

namespace Fga.Net.DependencyInjection.Http;

internal sealed class InjectableBaseClient : BaseClient
{
    public InjectableBaseClient(IOptions<OpenFga.Sdk.Configuration.Configuration> configuration, HttpClient httpClient) : base(configuration.Value, httpClient)
    {
    }
}