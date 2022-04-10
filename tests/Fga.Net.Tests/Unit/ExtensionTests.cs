using System;
using Auth0.Fga.Api;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fga.Net.Tests.Unit
{
    public class ExtensionTests
    {
        [Fact]
        public void ClientExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddAuth0FgaClient(x =>
            {
                x.StoreId = Guid.NewGuid().ToString();
                x.ClientId = Guid.NewGuid().ToString();
                x.ClientSecret = Guid.NewGuid().ToString();
            });

            var provider = collection.BuildServiceProvider();

            var authClient = provider.GetService<Auth0FgaApi>();
            Assert.NotNull(authClient);
            Assert.IsType<InjectableAuth0FgaApi>(authClient);
        }

        [Fact]
        public void AspNetCoreServiceExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddAuth0Fga(x =>
            {
                x.StoreId = Guid.NewGuid().ToString();
                x.ClientId = Guid.NewGuid().ToString();
                x.ClientSecret = Guid.NewGuid().ToString();
            });

            var provider = collection.BuildServiceProvider();

            var col = provider.GetServices<IAuthorizationHandler>();

            Assert.Contains(col, handler => handler.GetType() == typeof(FineGrainedAuthorizationHandler));

            var authClient = provider.GetService<Auth0FgaApi>();
            Assert.NotNull(authClient);
            Assert.IsType<InjectableAuth0FgaApi>(authClient);

        }

        [Fact]
        public void AuthorizationPolicyExtension_RegisterCorrectly()
        {
            var policy = new AuthorizationPolicyBuilder().AddFgaRequirement().Build();

            Assert.Contains(policy.Requirements, requirement => requirement.GetType() == typeof(FineGrainedAuthorizationRequirement));
        }

    }
}
