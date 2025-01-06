using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.DependencyInjection;
using Fga.Net.DependencyInjection.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;
using Xunit;

namespace Fga.Net.Tests.Unit;

public class ExtensionTests
{
    public static TheoryData<ExtensionScenario> BadExtensions =>
    [
        new ExtensionScenario("Empty Extension Config - Auth0 FGA",
            config => config.ConfigureAuth0Fga(x => { })),

        new ExtensionScenario("Empty Schema",
            config => config.ConfigureOpenFga(x => { x.SetConnection("localhost"); })),

        new ExtensionScenario("Empty API Key", config => config.ConfigureOpenFga(x =>
        {
            x.SetConnection("http://localhost")
                .WithApiKeyAuthentication("");
        })),

        new ExtensionScenario("Empty OIDC configuration", config => config.ConfigureOpenFga(x =>
        {
            x.SetConnection("http://localhost")
                .WithOidcAuthentication("clientId", "", "issuer", "audience");
        }))

    ];


    [Fact]
    public void InvalidConfiguration_ThrowsException()
    {
        var collection = new ServiceCollection();

        collection.AddOpenFgaClient(config =>
        {
            config.SetStoreId(Guid.NewGuid().ToString());

            config.ConfigureAuth0Fga(x => { });
        });
    }
        

        
    [Theory]
    [MemberData(nameof(WorkingExtensions))]
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
        Assert.NotNull(apiClient);
        Assert.IsType<InjectableFgaApi>(apiClient);

        var fgaClient = provider.GetService<OpenFgaClient>();
        Assert.NotNull(fgaClient);
        Assert.IsType<InjectableFgaClient>(fgaClient);
    }


    [Theory]
    [MemberData(nameof(WorkingExtensions))]
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

        Assert.Contains(col, handler => handler is FineGrainedAuthorizationHandler);
    }

    public static TheoryData<ExtensionScenario> WorkingExtensions =>
        new()
        {
            new ExtensionScenario("Auth0 FGA", 
                config => config.ConfigureAuth0Fga(x =>
                {
                    x.SetEnvironment(FgaEnvironment.AU)
                        .WithAuthentication(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                })),
            new ExtensionScenario("OpenFGA - No Credentials",
                config => config.ConfigureOpenFga(x =>
                { x.SetConnection("http://localhost"); 
                        
                })),
            new ExtensionScenario("OpenFGA - API Key Auth", 
                config => config.ConfigureOpenFga(x =>
                {
                    x.SetConnection("http://localhost")
                        .WithApiKeyAuthentication("my-special-key");
                })),
            new ExtensionScenario("OpenFGA - OIDC Auth", 
                config => config.ConfigureOpenFga(x =>
                {
                    x.SetConnection("http://localhost")
                        .WithOidcAuthentication("clientId", "clientSecret", "issuer", "audience");
                })),
        };

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


    [Fact]
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
        
        Assert.Null(config.Credentials);
        Assert.Equal(openFgaUrl, config.ApiUrl);

    }

}