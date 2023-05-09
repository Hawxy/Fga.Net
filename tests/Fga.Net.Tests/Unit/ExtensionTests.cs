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
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Client;
using Xunit;
using Xunit.Abstractions;

namespace Fga.Net.Tests.Unit;

public class ExtensionTests
{
    public static TheoryData<Action<FgaConfigurationRoot>> BadExtensions =>
        new()
        {
            config => config.ConfigureAuth0Fga(x =>
            {
                    
            }),
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection("", "localhost");
            }),
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection(HttpScheme.Https, "localhost")
                    .WithApiKeyAuthentication("");
            }),
            config => config.ConfigureOpenFga(x =>
            {
                x.SetConnection(HttpScheme.Https, "localhost")
                    .WithOidcAuthentication("clientId", "", "issuer", "audience");
            }),
        };

    [Theory]
    [MemberData(nameof(BadExtensions))]
    public void InvalidConfiguration_ThrowsException(Action<FgaConfigurationRoot> extension)
    {
        var collection = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() =>
            collection.AddOpenFgaClient(config =>
            {
                config.StoreId = Guid.NewGuid().ToString();

                extension(config);
            }));
    }
        

        
    [Theory]
    [MemberData(nameof(WorkingExtensions))]
    public void ClientExtensions_RegisterCorrectly(ExtensionScenario scenario)
    {
        var collection = new ServiceCollection();

        collection.AddOpenFgaClient(config =>
        {
            config.StoreId = Guid.NewGuid().ToString();

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
            config.StoreId = Guid.NewGuid().ToString();
            scenario.Configuration(config);
        });

        collection.AddOpenFgaMiddleware(x =>
        {
            x.UserIdentityResolver = principal => principal.Identity!.Name!;
        });

        var provider = collection.BuildServiceProvider();

        var col = provider.GetServices<IAuthorizationHandler>();

        Assert.Contains(col, handler => handler is FineGrainedAuthorizationHandler);
    }

    public class ExtensionScenario : IXunitSerializable 
    {
        public ExtensionScenario()
        {
        }

        public ExtensionScenario(string description, Action<FgaConfigurationRoot> configuration)
        {
            Description = description;
            Configuration = configuration;
        }

        public override string ToString()
        {
            return Description;
        }

        public void Deserialize(IXunitSerializationInfo info)
        { }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Description), Description);
        }

        public string Description { get; init; }
        public Action<FgaConfigurationRoot> Configuration { get; }

    }

    public static TheoryData<ExtensionScenario> WorkingExtensions =>
        new()
        {
            new ExtensionScenario("Auth0 FGA", 
                config => config.ConfigureAuth0Fga(x =>
                {
                    x.WithAuthentication(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                })),
            new ExtensionScenario("OpenFGA - No Credentials",
                config => config.ConfigureOpenFga(x =>
                { x.SetConnection(HttpScheme.Http, "localhost"); 
                        
                })),
            new ExtensionScenario("OpenFGA - API Key Auth", 
                config => config.ConfigureOpenFga(x =>
                {
                    x.SetConnection(HttpScheme.Https, "localhost")
                        .WithApiKeyAuthentication("my-special-key");
                })),
            new ExtensionScenario("OpenFGA - OIDC Auth", 
                config => config.ConfigureOpenFga(x =>
                {
                    x.SetConnection(HttpScheme.Https, "localhost")
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

}