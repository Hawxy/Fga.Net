using Alba;
using Fga.Net.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using OpenFga.Sdk.Client.Model;
using TUnit.Core.Interfaces;

namespace Fga.Net.Tests.Middleware;

public class WebAppFixture : IAsyncInitializer, IAsyncDisposable
{
    public IAlbaHost AlbaHost = null!;

    public async Task InitializeAsync()
    {
        var authorizationClientMock = new Mock<IFgaCheckDecorator>();

        authorizationClientMock.Setup(c =>
                c.BatchCheck(It.IsAny<ClientBatchCheckRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientBatchCheckRequest res, CancellationToken _) =>
            {
                var entry = res.Checks.First();
                return entry.User == $"user:{MockJwtConfiguration.DefaultUser}"
                    ? new ClientBatchCheckResponse() { Result = [new() { Allowed = true, Request = entry }] }
                    : new ClientBatchCheckResponse() { Result = [new() { Allowed = false, Request = entry }] };
            });

        AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(s =>
            {
                s.Replace(ServiceDescriptor.Scoped<IFgaCheckDecorator>(_ => authorizationClientMock.Object));
            });

        }, MockJwtConfiguration.GetDefaultStubConfiguration());
    }

    public async ValueTask DisposeAsync()
    {
        if (AlbaHost is not null)
            await AlbaHost.DisposeAsync();
    }
}

public abstract class WebAppBase(WebAppFixture fixture)
{
    protected IAlbaHost Host => fixture.AlbaHost;
}