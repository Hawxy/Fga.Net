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
            c.CheckAsync(It.IsAny<string>(),
                It.IsAny<CheckRequestParams>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, CheckRequestParams res, CancellationToken _) => 
                res.Tuple_key!.User == MockJwtConfiguration.DefaultUser 
                    ? new CheckResponse() { Allowed = true } 
                    : new CheckResponse() { Allowed = false });


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