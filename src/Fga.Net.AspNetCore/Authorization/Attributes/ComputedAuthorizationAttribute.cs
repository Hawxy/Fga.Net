using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class ComputedAuthorizationAttribute : Attribute
{
    public abstract ValueTask<string> GetObject(HttpContext context);
    public abstract ValueTask<string> GetRelation(HttpContext context);
    public abstract ValueTask<string> GetUser(HttpContext context);
}

