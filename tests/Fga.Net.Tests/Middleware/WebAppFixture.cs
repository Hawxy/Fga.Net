using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alba;
using Fga.Net.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Model;
using Xunit;

namespace Fga.Net.Tests.Middleware;

public class WebAppFixture : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        var authorizationClientMock = new Mock<IFgaCheckDecorator>();

        authorizationClientMock.Setup(c =>
                c.BatchCheck(It.IsAny<List<ClientCheckRequest>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<ClientCheckRequest> res, CancellationToken _) =>
            {
                var entry = res.First();
                return entry.User == MockJwtConfiguration.DefaultUser
                    ? new BatchCheckResponse() { Responses = new List<BatchCheckSingleResponse>() { new(true, entry) } }
                    : new BatchCheckResponse() { Responses = new List<BatchCheckSingleResponse>() { new(false, entry) } };
            });



        AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(s =>
            {
                s.Replace(ServiceDescriptor.Scoped<IFgaCheckDecorator>(_ => authorizationClientMock.Object));
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