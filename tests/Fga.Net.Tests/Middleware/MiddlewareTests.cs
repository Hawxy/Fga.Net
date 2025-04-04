﻿using System.Net;
using System.Security.Claims;
using Alba;

namespace Fga.Net.Tests.Middleware;

[ClassDataSource<WebAppFixture>(Shared = SharedType.PerTestSession)]
public class MiddlewareTests(WebAppFixture fixture) : WebAppBase(fixture)
{
    [Test]
    public async Task Authorization_HappyPath_Succeeds()
    {
        await Host.Scenario(_ =>
        {
            _.Get.Url($"/test/document/{Guid.NewGuid()}");
            _.StatusCodeShouldBeOk();
        });
    }
    
    [Test]
    public async Task Authorization_UnhappyPath_Forbidden()
    {
        await Host.Scenario(_ =>
        {
            _.RemoveClaim(ClaimTypes.NameIdentifier);
            _.WithClaim(new Claim(ClaimTypes.NameIdentifier, MockJwtConfiguration.FakeUser));
            _.Get.Url($"/test/document/{Guid.NewGuid()}");
            _.StatusCodeShouldBe(HttpStatusCode.Forbidden);
        });
    }
    
    [Test]
    public async Task Authorization_IgnoredPath_Succeeds()
    {
        await Host.Scenario(_ =>
        {
            _.RemoveClaim(ClaimTypes.NameIdentifier);
            _.WithClaim(new Claim(ClaimTypes.NameIdentifier, MockJwtConfiguration.FakeUser));
            _.Get.Url("/test/ignored");
            _.StatusCodeShouldBeOk();
        });
    }
}