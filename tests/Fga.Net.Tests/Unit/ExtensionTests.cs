using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using Xunit;

namespace Fga.Net.Tests.Unit
{
    public class ExtensionTests
    {
        [Fact]
        public void ClientExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddOpenFgaClient(x =>
            {
                x.StoreId = Guid.NewGuid().ToString();
                x.WithAuth0FgaDefaults(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            });

            var provider = collection.BuildServiceProvider();

            var apiClient = provider.GetService<OpenFgaApi>();
            Assert.NotNull(apiClient);
            Assert.IsType<InjectableFgaApi>(apiClient);

            var fgaClient = provider.GetService<OpenFgaClient>();
            Assert.NotNull(fgaClient);
            Assert.IsType<InjectableFgaClient>(fgaClient);
        }

        [Fact]
        public void AspNetCoreServiceExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddOpenFgaClient(x =>
            {
                x.StoreId = Guid.NewGuid().ToString();
                x.WithAuth0FgaDefaults(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            });

            collection.AddOpenFgaMiddleware(x =>
            {
                x.UserIdentityResolver = principal => principal.Identity!.Name!;
            });

            var provider = collection.BuildServiceProvider();

            var col = provider.GetServices<IAuthorizationHandler>();

            Assert.Contains(col, handler => handler is FineGrainedAuthorizationHandler);
        }

        [Fact]
        public void AuthorizationPolicyExtension_RegisterCorrectly()
        {
            var policy = new AuthorizationPolicyBuilder().AddFgaRequirement().Build();

            Assert.Contains(policy.Requirements, requirement => requirement is FineGrainedAuthorizationRequirement);
        }

        [Fact]
        public void MinimalExtensions_RegisterAttributesCorrectly()
        {
            var builder = new TestEndpointRouteBuilder();
            builder.MapGet("/", () => Task.CompletedTask)
                .WithFgaHeaderCheck("x", "y", "z")
                .WithFgaRouteCheck("x", "y", "z")
                .WithFgaQueryCheck("x", "y", "z")
                .WithFgaPropertyCheck("x", "y", "z");


            var endpoint = builder.DataSources.Single().Endpoints.Single();

            var metadata = endpoint.Metadata.GetOrderedMetadata<FgaAttribute>();
            
            Assert.Equal(4, metadata.Count);
            Assert.Contains(metadata, attribute => attribute is FgaHeaderObjectAttribute);
            Assert.Contains(metadata, attribute => attribute is FgaRouteObjectAttribute);
            Assert.Contains(metadata, attribute => attribute is FgaQueryObjectAttribute);
            Assert.Contains(metadata, attribute => attribute is FgaPropertyObjectAttribute);

        }

        private class TestEndpointRouteBuilder : IEndpointRouteBuilder
        {
            public IServiceProvider ServiceProvider { get; } = new ServiceCollection().BuildServiceProvider();
            public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();
            public IApplicationBuilder CreateApplicationBuilder()
            {
                throw new NotImplementedException();
            }
        }

    }
}
