using System.Security.Claims;
using Alba.Security;

namespace Fga.Net.Tests.Middleware;

public static class MockJwtConfiguration
{
    public const string DefaultUser = "auth0|real-fake-user";
    public const string FakeUser = "fake-user";
    public const string Issuer = "https://hawxy.au.auth0.com/";

    private static readonly Claim[] DefaultClaims = {
        new("iss", Issuer),
        new(ClaimTypes.NameIdentifier, DefaultUser),
    };

    public static JwtSecurityStub GetDefaultStubConfiguration()
    {
        var stub = new JwtSecurityStub();
        foreach (var defaultClaim in DefaultClaims)
        {
            stub.With(defaultClaim);
        }

        stub.WithName(DefaultUser);

        return stub;
    }
}