using System.Linq;
using System.Threading.Tasks;
using Alba;
using Auth0.Fga.Api;
using Auth0.Fga.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fga.Net.Tests.Client
{
    [Collection(nameof(EndpointWebAppCollection))]
    public class EndpointTests
    {
        private readonly IAlbaHost _host;

        public EndpointTests(EndpointWebAppFixture fixture)
        {
            _host = fixture.AlbaHost;
        }

        [Fact()]
        private async Task GetEndpoints_Return_200()
        {
            using var scope = _host.Services.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<Auth0FgaApi>();
            var modelsResponse = await client.ReadAuthorizationModels();

            Assert.NotNull(modelsResponse);
            Assert.NotNull(modelsResponse.AuthorizationModels);
            Assert.True(modelsResponse.AuthorizationModels?.Count > 0);

            var modelId = modelsResponse.AuthorizationModels?.First().Id!;

            var modelResponse = await client.ReadAuthorizationModel(modelId);

            Assert.NotNull(modelResponse);
            Assert.NotNull(modelResponse.AuthorizationModel.Id);

            var assertions = await client.ReadAssertions(modelId);

            Assert.NotNull(assertions);
            Assert.True(assertions.Assertions?.Count > 0);
            var assertion = assertions.Assertions!.First().TupleKey;

            Assert.NotEmpty(assertion.Object!);
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

    }
}
