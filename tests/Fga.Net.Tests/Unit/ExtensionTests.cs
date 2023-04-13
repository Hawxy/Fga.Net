using System;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
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

    }
}
