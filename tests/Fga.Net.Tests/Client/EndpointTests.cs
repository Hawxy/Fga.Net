using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alba;
using Microsoft.Extensions.DependencyInjection;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Model;
using Xunit;

namespace Fga.Net.Tests.Client;

[Collection(nameof(EndpointWebAppCollection))]
public class EndpointTests
{
    private readonly IAlbaHost _host;

    public EndpointTests(EndpointWebAppFixture fixture)
    {
        _host = fixture.AlbaHost;
    }

    [Fact]
    private async Task GetEndpoints_OpenFgaApi_Return_200()
    {
        using var scope = _host.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OpenFgaApi>();
        var modelsResponse = await client.ReadAuthorizationModels();

        Assert.NotNull(modelsResponse);
        Assert.NotNull(modelsResponse.AuthorizationModels);
        Assert.True(modelsResponse.AuthorizationModels?.Count > 0);

        var modelId = modelsResponse.AuthorizationModels?.First().Id!;

        var modelResponse = await client.ReadAuthorizationModel(modelId);

        Assert.NotNull(modelResponse);
        Assert.NotNull(modelResponse.AuthorizationModel?.Id);

        var assertions = await client.ReadAssertions(modelId);

        Assert.NotNull(assertions);
        Assert.True(assertions.Assertions?.Count > 0);
        var assertion = assertions.Assertions!.First().TupleKey;

        Assert.NotEmpty(assertion!.Object!);
        Assert.NotEmpty(assertion.Relation!);
        Assert.NotEmpty(assertion.User!);
            
        var graph = await client.Expand(new ExpandRequest()
        {
            AuthorizationModelId = modelId,
            TupleKey = assertion
        });

        Assert.NotNull(graph.Tree);
        Assert.NotNull(graph.Tree!.Root!.Name);

        var watch = await client.ReadChanges();
        Assert.NotNull(watch);


    }

    [Fact]
    private async Task GetEndpoints_OpenFgaClient_Return_200()
    {
        using var scope = _host.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OpenFgaClient>();
        var modelsResponse = await client.ReadAuthorizationModels();

        Assert.NotNull(modelsResponse);
        Assert.NotNull(modelsResponse.AuthorizationModels);
        Assert.True(modelsResponse.AuthorizationModels?.Count > 0);
            
        var modelId = modelsResponse.AuthorizationModels?.First().Id!;

        var modelResponse = await client.ReadAuthorizationModel(new ClientReadAuthorizationModelOptions() {AuthorizationModelId = modelId});

        Assert.NotNull(modelResponse);
        Assert.NotNull(modelResponse.AuthorizationModel?.Id);

        var assertions = await client.ReadAssertions(new ClientReadAssertionsOptions() { AuthorizationModelId = modelId});

        Assert.NotNull(assertions);
        Assert.True(assertions.Assertions?.Count > 0);
        var assertion = assertions.Assertions!.First().TupleKey;

        Assert.NotEmpty(assertion!.Object!);
        Assert.NotEmpty(assertion.Relation!);
        Assert.NotEmpty(assertion.User!);

        var graph = await client.Expand(new ClientExpandRequest()
        {
            Object = assertion.Object!,
            Relation = assertion.Relation!
        });

        Assert.NotNull(graph.Tree);
        Assert.NotNull(graph.Tree!.Root!.Name);

        var watch = await client.ReadChanges(new ClientReadChangesRequest() {Type = "document"});
        Assert.NotNull(watch);
    }

}