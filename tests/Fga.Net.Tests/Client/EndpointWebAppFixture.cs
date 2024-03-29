﻿using System.Threading.Tasks;
using Alba;
using Fga.Net.Tests.Middleware;
using Xunit;

namespace Fga.Net.Tests.Client;

public class EndpointWebAppFixture : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        AlbaHost = await Alba.AlbaHost.For<Program>(_ => { }, MockJwtConfiguration.GetDefaultStubConfiguration());
    }

    public async Task DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
    }
}

[CollectionDefinition(nameof(EndpointWebAppCollection))]
public class EndpointWebAppCollection : ICollectionFixture<EndpointWebAppFixture>
{
}