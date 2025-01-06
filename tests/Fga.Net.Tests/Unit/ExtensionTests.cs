using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.DependencyInjection;
using Fga.Net.DependencyInjection.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;

namespace Fga.Net.Tests.Unit;

public static class ExtensionTestDataSource
{
    public static IEnumerable<Func<ExtensionScenario>> BadExtensions()
    {
        yield return () => new ExtensionScenario("Empty Extension Config - Auth0 FGA",
            config => config.ConfigureAuth0Fga(x => { }));

        yield return () => new ExtensionScenario("Empty Schema",
            config => config.ConfigureOpenFga(x => { x.SetConnection("localhost"); }));

        yield return () => new ExtensionScenario("Empty API Key", config => config.ConfigureOpenFga(x =>
        {
            x.SetConnection("http://localhost")
                .WithApiKeyAuthentication("");
        }));

        yield return () => new ExtensionScenario("Empty OIDC configuration", config => config.ConfigureOpenFga(x =>
        {
            x.SetConnection("http://localhost")
                .WithOidcAuthentication("clientId", "", "issuer", "audience");
        }));
    }

    public static IEnumerable<Func<ExtensionScenario>> WorkingExtensions()
    {
        yield return () => new ExtensionScenario("Auth0 FGA",
            config => config.ConfigureAuth0Fga(x =>
            {
                x.SetEnvironment(FgaEnvironment.AU)
                    .WithAuthentication(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }));

        yield return () => new ExtensionScenario("Auth0 FGA",
            config => config.ConfigureAuth0Fga(x =>
            {
                x.SetEnvironment(FgaEnvironment.AU)
                    .WithAuthentication(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }));
        yield return () => new ExtensionScenario("OpenFGA - No Credentials",
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection("http://localhost");

            }));
        yield return () => new ExtensionScenario("OpenFGA - API Key Auth",
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection("http://localhost")
                    .WithApiKeyAuthentication("my-special-key");
            }));
        yield return () => new ExtensionScenario("OpenFGA - OIDC Auth",
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection("http://localhost")
                    .WithOidcAuthentication("clientId", "clientSecret", "issuer", "audience");
            }));
    }
}
public class ExtensionTests
{
    [Test]
    [MethodDataSource(typeof(ExtensionTestDataSource), nameof(ExtensionTestDataSource.BadExtensions))]
    public void InvalidConfiguration_ThrowsException(ExtensionScenario scenario)
    {
        var collection = new ServiceCollection();

        collection.Invoking(x => x.AddOpenFgaClient(config =>
        {
            config.SetStoreId(Guid.NewGuid().ToString());

            scenario.Configuration(config);
        }))
            .Should()
            .ThrowExactly<InvalidOperationException>();
    }
    
    [Test]
    [MethodDataSource(typeof(ExtensionTestDataSource), nameof(ExtensionTestDataSource.WorkingExtensions))]
    public void ClientExtensions_RegisterCorrectly(ExtensionScenario scenario)
    {
        var collection = new ServiceCollection();

        collection.AddOpenFgaClient(config =>
        {
            config.SetStoreId(Ulid.NewUlid().ToString()); 

            scenario.Configuration(config);

        });

        var provider = collection.BuildServiceProvider();

        var apiClient = provider.GetService<OpenFgaApi>();
        apiClient.Should().NotBeNull();
        apiClient.Should().BeOfType<InjectableFgaApi>();

        var fgaClient = provider.GetService<OpenFgaClient>();
        fgaClient.Should().NotBeNull();
        fgaClient.Should().BeOfType<InjectableFgaClient>();
    }


    [Test]
    [MethodDataSource(typeof(ExtensionTestDataSource), nameof(ExtensionTestDataSource.WorkingExtensions))]
    public void AspNetCoreServiceExtensions_RegisterCorrectly(ExtensionScenario scenario)
    {
        var collection = new ServiceCollection();

        collection.AddOpenFgaClient(config =>
        {
            config.SetStoreId(Ulid.NewUlid().ToString()); 
            scenario.Configuration(config);
        });

        collection.AddOpenFgaMiddleware(x =>
        {
            x.SetUserIdentifier("user", principal => principal.Identity!.Name!);
        });

        var provider = collection.BuildServiceProvider();

        var col = provider.GetServices<IAuthorizationHandler>();

        col.Should().Contain(handler => handler is FineGrainedAuthorizationHandler);
    }

    [Test]
    public void AuthorizationPolicyExtension_RegisterCorrectly()
    {
        var policy = new AuthorizationPolicyBuilder().AddFgaRequirement().Build();

        policy.Requirements.Should().Contain(requirement => requirement is FineGrainedAuthorizationRequirement);
    }

    [Test]
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
        metadata.Count.Should().Be(4);
        metadata.Should().Contain(attribute => attribute is FgaHeaderObjectAttribute);
        metadata.Should().Contain(attribute => attribute is FgaRouteObjectAttribute);
        metadata.Should().Contain(attribute => attribute is FgaQueryObjectAttribute);
        metadata.Should().Contain(attribute => attribute is FgaPropertyObjectAttribute);
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


    [Test]
    public void PostConfigureOptions_OverridesSettings()
    {
        var collection = new ServiceCollection();

        collection.AddOpenFgaClient(config =>
        {
            config.SetStoreId(Guid.NewGuid().ToString()); 

            config.ConfigureAuth0Fga(x=> x.WithAuthentication("FgaClientId", "FgaClientSecret"));

        });

        var openFgaUrl = "http://localhost:8080";
        collection.PostConfigureFgaClient(config =>
        {
            config.SetStoreId(Guid.NewGuid().ToString());
            config.ConfigureOpenFga(x =>
            {
                x.SetConnection(openFgaUrl);
            });
        });

        var provider = collection.BuildServiceProvider();

        var config = provider.GetRequiredService<IOptions<FgaClientConfiguration>>().Value;

        config.Credentials.Should().BeNull();
        config.ApiUrl.Should().Be(openFgaUrl);
    }

}