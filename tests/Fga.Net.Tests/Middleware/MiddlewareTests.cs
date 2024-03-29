﻿using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Alba;
using Xunit;

namespace Fga.Net.Tests.Middleware;

[Collection(nameof(WebAppCollection))]
public class MiddlewareTests
{
    private readonly IAlbaHost _alba;

    public MiddlewareTests(WebAppFixture fixture)
    {
        _alba = fixture.AlbaHost;
    }
    [Fact]
    public async Task Authorization_HappyPath_Succeeds()
    {
        await _alba.Scenario(_ =>
        {
            _.Get.Url($"/test/{Guid.NewGuid()}");
            _.StatusCodeShouldBeOk();
        });
    }
    [Fact]
    public async Task Authorization_UnhappyPath_Forbidden()
    {
        await _alba.Scenario(_ =>
        {
            _.RemoveClaim(ClaimTypes.NameIdentifier);
            _.WithClaim(new Claim(ClaimTypes.NameIdentifier, MockJwtConfiguration.FakeUser));
            _.Get.Url($"/test/{Guid.NewGuid()}");
            _.StatusCodeShouldBe(HttpStatusCode.Forbidden);
        });
    }
}