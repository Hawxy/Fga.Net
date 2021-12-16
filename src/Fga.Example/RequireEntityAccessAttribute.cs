using Fga.Net.AspNetCore.Authorization;

namespace Fga.Example;

public class RequireEntityAccessAttribute : ComputedAuthorizeAttribute
{
    public RequireEntityAccessAttribute(string prefix = "document", string routeValue = "documentId")
        : base(context => $"{prefix}:{context.GetRouteValue(routeValue)}", GetOperationByRoute, context => context.User.Identity!.Name!)
    {
    }

    private static string GetOperationByRoute(HttpContext context)
    {
        return context.Request.Method switch
        {
            "GET" => "reader",
            "POST" => "writer",
            _ => "owner"
        };
    }
}