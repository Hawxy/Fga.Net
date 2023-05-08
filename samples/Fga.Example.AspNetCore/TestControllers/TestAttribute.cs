using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Example.AspNetCore.TestControllers;

public class TestAuthorizationAttribute : FgaBaseObjectAttribute
{
    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult("fake-relation");
    }

    public override ValueTask<string> GetObject(HttpContext context)
    {
        return ValueTask.FromResult("test:test");
    }
}