using Fga.Net.AspNetCore.Authorization;

namespace Fga.Example;

public class DangerHardcodedAuthorizeAttribute : ComputedAuthorizeAttribute
{
    public DangerHardcodedAuthorizeAttribute(string @object, string relation,string user) : base(_ => @object, _=> relation, _ => user)
    {
    }
}

public class EntityAuthorizationAttribute : ComputedAuthorizeAttribute
{
    public EntityAuthorizationAttribute(string prefix = "document", string routeValue = "documentId")
        : base(context => $"{prefix}:{context.GetRouteValue(routeValue)}", GetOperationByRoute, context => context.User.Identity!.Name!)
    {
    }

    private static string GetOperationByRoute(HttpContext context)
    {
        return context.Request.Method switch
        {
            "GET" => "read",
            "POST" => "write",
            _ => "owner"
        };
    }
}