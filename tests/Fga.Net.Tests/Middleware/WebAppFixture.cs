using System.Threading;
using System.Threading.Tasks;
using Alba;
using Auth0.Fga.Api;
using Auth0.Fga.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace Fga.Net.Tests.Middleware;

public class WebAppFixture : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        var authorizationClientMock = new Mock<Auth0FgaApi>();

        authorizationClientMock.Setup(c =>
            c.Check(It.IsAny<CheckRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, CheckRequest res, CancellationToken _) => 
                res.TupleKey!.User == MockJwtConfiguration.DefaultUser 
                    ? new CheckResponse() { Allowed = true } 
                    : new CheckResponse() { Allowed = false });


        AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(s =>
            {
                s.Replace(ServiceDescriptor.Scoped<Auth0FgaApi>(_ => authorizationClientMock.Object));
            });

        }, MockJwtConfiguration.GetDefaultStubConfiguration());
    }

    public async Task DisposeAsync()
    {
        if (AlbaHost is not null)
            await AlbaHost.DisposeAsync();
    }
}

[CollectionDefinition(nameof(WebAppCollection))]
public class WebAppCollection : ICollectionFixture<WebAppFixture>
{
}