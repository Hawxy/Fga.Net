using Alba;
using Fga.Net.Tests.Middleware;
using TUnit.Core.Interfaces;

namespace Fga.Net.Tests.Client;

public class EndpointWebAppFixture : IAsyncInitializer, IAsyncDisposable
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        AlbaHost = await Alba.AlbaHost.For<Program>(_ => { }, MockJwtConfiguration.GetDefaultStubConfiguration());
    }

    public async ValueTask DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
    }
}

public abstract class EndpointWebAppBase(EndpointWebAppFixture fixture)
{
    protected IAlbaHost Host => fixture.AlbaHost;
}