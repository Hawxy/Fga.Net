using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

public class StringComputedAuthorizationAttribute : ComputedAuthorizationAttribute
{
    private readonly string _user;
    private readonly string _relation;
    private readonly string _object;
    public StringComputedAuthorizationAttribute(string user, string relation, string @object)
    {
        _user = user;
        _relation = relation;
        _object = @object;
    }
    public override ValueTask<string> GetUser(HttpContext context) => ValueTask.FromResult(_user);

    public override ValueTask<string> GetRelation(HttpContext context) => ValueTask.FromResult(_relation);

    public override ValueTask<string> GetObject(HttpContext context) => ValueTask.FromResult(_object);
}