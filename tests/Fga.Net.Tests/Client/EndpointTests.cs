using System.Linq;
using System.Threading.Tasks;
using Alba;
using Fga.Net.Authorization;
using Fga.Net.Tests.Middleware;
using Microsoft.Extensions.Configuration;
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


        [Fact]
        private async Task GetEndpoints_Return_200()
        {
            using var scope = _host.Services.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<IFgaAuthorizationClient>();
            var storeId = scope.ServiceProvider.GetRequiredService<IConfiguration>()["Auth0Fga:StoreId"];
            var modelIds = await client.ReadAuthorizationModelsAsync(storeId);

            Assert.NotNull(modelIds);
            Assert.NotNull(modelIds.Authorization_model_ids);
            Assert.True(modelIds.Authorization_model_ids?.Count > 0);

            var modelId = modelIds.Authorization_model_ids?.First()!;
            var assertions = await client.ReadAssertionsAsync(storeId, modelId);

            Assert.NotNull(assertions);
            Assert.True(assertions.Assertions?.Count > 0);
            var assertion = assertions.Assertions!.First().Tuple_key;

            Assert.NotEmpty(assertion.Object!);
            Assert.NotEmpty(assertion.Relation!);
            Assert.NotEmpty(assertion.User!);

            /* Disabled due to this endpoint returning 400 when no settings
            var settings = await client.ReadSettingsAsync(storeId);
            Assert.NotNull(settings);
            Assert.True(settings.Environment == Environment.DEVELOPMENT);*/


            var graph = await client.ExpandAsync(storeId, new ExpandRequestParams()
            {
                Authorization_model_id = modelId,
                Tuple_key = assertion
            });

            Assert.NotNull(graph);
            Assert.NotNull(graph.Tree!.Root!.Name);
            Assert.NotNull(graph.Tree!.Root!.Union!.Nodes1);

        }

    }
}
