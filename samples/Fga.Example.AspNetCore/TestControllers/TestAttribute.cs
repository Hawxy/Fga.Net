using Fga.Net.AspNetCore.Authorization.Attributes;

namespace Fga.Example.AspNetCore.TestControllers;

public class TestAuthorizationAttribute : FgaAttribute
{

    public override ValueTask<string> GetUser(HttpContext context)
    {
        return ValueTask.FromResult(context.User.Identity!.Name!);
    }

    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult("fake-relation");
    }

    public override ValueTask<string> GetObject(HttpContext context)
    {
        return ValueTask.FromResult("test:test");
    }
}