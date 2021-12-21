using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.Authentication;
using Fga.Net.Authorization;
using Fga.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fga.Net.Tests
{
    public class ExtensionTests
    {
        [Fact]
        public void ClientExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddAuth0FgaAuthorizationClient(x => { });
            collection.AddAuth0FgaAuthenticationClient();

            var provider = collection.BuildServiceProvider();

            var authClient = provider.GetService<IFgaAuthenticationClient>();
            Assert.NotNull(authClient);
            Assert.IsType<FgaAuthenticationClient>(authClient);

            var authorizationClient = provider.GetService<IFgaAuthorizationClient>();
            Assert.NotNull(authorizationClient);
            Assert.IsType<FgaAuthorizationClient>(authorizationClient);

            var tokenHandler = provider.GetService<FgaTokenHandler>();

            Assert.NotNull(tokenHandler);
        }

        [Fact]
        public void AspNetCoreServiceExtensions_RegisterCorrectly()
        {
            var collection = new ServiceCollection();

            collection.AddAuth0Fga(x => { });

            var provider = collection.BuildServiceProvider();

            var col = provider.GetServices<IAuthorizationHandler>();

            Assert.Contains(col, handler => handler.GetType() == typeof(SandcastleAuthorizationHandler));

            var authClient = provider.GetService<IFgaAuthenticationClient>();
            Assert.NotNull(authClient);
            Assert.IsType<FgaAuthenticationClient>(authClient);

            var authorizationClient = provider.GetService<IFgaAuthorizationClient>();
            Assert.NotNull(authorizationClient);
            Assert.IsType<FgaAuthorizationClient>(authorizationClient);

            var tokenHandler = provider.GetService<FgaTokenHandler>();

            Assert.NotNull(tokenHandler);

        }

        [Fact]
        public void AuthorizationPolicyExtension_RegisterCorrectly()
        {
            var policy = new AuthorizationPolicyBuilder().AddFgaRequirement().Build();

            Assert.Contains(policy.Requirements, requirement => requirement.GetType() == typeof(SandcastleRequirement));
        }

    }
}
