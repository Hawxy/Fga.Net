using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ComputedAuthorizeAttribute : Attribute
{
    public ComputedAuthorizeAttribute(Func<HttpContext, string> @object, Func<HttpContext, string> relation, Func<HttpContext, string> user)
    {
        Object = @object;
        Relation = relation;
        User = user;
    }

    public Func<HttpContext, string> Object { get; }
    public Func<HttpContext, string> Relation { get; }
    public Func<HttpContext, string> User { get; }

}