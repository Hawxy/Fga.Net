using System.Threading;
using System.Threading.Tasks;
using Alba;
using Fga.Net.Authorization;
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
        var authorizationClientMock = new Mock<IFgaAuthorizationClient>();

        authorizationClientMock.Setup(c =>
            c.CheckAsync(
                It.IsAny<CheckTupleRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CheckTupleRequest res, CancellationToken _) => 
                res.TupleKey.User == MockJwtConfiguration.DefaultUser 
                    ? new CheckTupleResponse() { Allowed = true } 
                    : new CheckTupleResponse() { Allowed = false });


        AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(s =>
            {
                s.Replace(ServiceDescriptor.Scoped<IFgaAuthorizationClient>(_ => authorizationClientMock.Object));
            });

        }, MockJwtConfiguration.GetDefaultStubConfiguration());
    }

    public async Task DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
    }
}

[CollectionDefinition(nameof(WebAppCollection))]
public class WebAppCollection : ICollectionFixture<WebAppFixture>
{
}