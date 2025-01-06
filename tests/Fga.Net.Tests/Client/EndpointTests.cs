using Fga.Net.DependencyInjection.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;

namespace Fga.Net.Tests.Client;

[ClassDataSource<EndpointWebAppFixture>(Shared = SharedType.PerAssembly)]
public class EndpointTests(EndpointWebAppFixture fixture) : EndpointWebAppBase(fixture)
{
    [Test]
    private async Task GetEndpoints_OpenFgaApi_Return_200()
    {
        using var scope = Host.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OpenFgaApi>();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<FgaClientConfiguration>>().Value;
        var modelsResponse = await client.ReadAuthorizationModels(config.StoreId!);
        
        modelsResponse.Should().NotBeNull();
        modelsResponse.AuthorizationModels.Should().NotBeNull();
        modelsResponse.AuthorizationModels.Count.Should().BePositive();
    }

    [Test]
    private async Task GetEndpoints_OpenFgaClient_Return_200()
    {
        using var scope = Host.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OpenFgaClient>();
        var modelsResponse = await client.ReadAuthorizationModels();

        modelsResponse.Should().NotBeNull();
        modelsResponse.AuthorizationModels.Should().NotBeNull();
        modelsResponse.AuthorizationModels.Count.Should().BePositive();
    }

}