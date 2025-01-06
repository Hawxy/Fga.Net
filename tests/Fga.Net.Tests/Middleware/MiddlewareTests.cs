using System.Net;
using System.Security.Claims;
using Alba;

namespace Fga.Net.Tests.Middleware;

[ClassDataSource<WebAppFixture>(Shared = SharedType.PerAssembly)]
public class MiddlewareTests(WebAppFixture fixture) : WebAppBase(fixture)
{
    [Test]
    public async Task Authorization_HappyPath_Succeeds()
    {
        await Host.Scenario(_ =>
        {
            _.Get.Url($"/test/{Guid.NewGuid()}");
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
            _.Get.Url($"/test/{Guid.NewGuid()}");
            _.StatusCodeShouldBe(HttpStatusCode.Forbidden);
        });
    }
}