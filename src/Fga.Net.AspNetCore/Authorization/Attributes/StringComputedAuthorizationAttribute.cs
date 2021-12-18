using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

public class StringComputedAuthorizationAttribute : ComputedAuthorizationAttribute
{
    private readonly string _object;
    private readonly string _relation;
    private readonly string _user;

    public StringComputedAuthorizationAttribute(string @object, string relation, string user)
    {
        _object = @object;
        _relation = relation;
        _user = user;
    }

    public override ValueTask<string> GetObject(HttpContext context) => ValueTask.FromResult(_object);

    public override ValueTask<string> GetRelation(HttpContext context) => ValueTask.FromResult(_relation);

    public override ValueTask<string> GetUser(HttpContext context) => ValueTask.FromResult(_user);
}