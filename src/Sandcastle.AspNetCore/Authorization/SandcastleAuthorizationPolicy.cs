using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Sandcastle.AspNetCore.Authorization;


public class SandcastleRequirement : IAuthorizationRequirement
{
}
// Think about if this should be a 1:1 handler, A IAuthorizationHandler or a requirement that implements its own handler.
public class SandcastleAuthorizationHandler : AuthorizationHandler<SandcastleRequirement>
{
    private readonly SandcastleAuthorizationClient _client;

    public SandcastleAuthorizationHandler(SandcastleAuthorizationClient client)
    {
        _client = client;
    }


    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SandcastleRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var attributes = endpoint!.Metadata.GetOrderedMetadata<SandcastleComputedAuthorizeAttribute>();
            // The user is enforcing the sandcastle policy but there's no attributes here, bit odd.
            if (attributes.Count == 0)
                return;
            foreach (var attribute in attributes)
            {
                var @object = attribute.Object.Invoke(httpContext);
                var relation = attribute.Relation.Invoke(httpContext);
                var user = attribute.User.Invoke(httpContext);

                var result = await _client.CheckAsync(new CheckRequest()
                {
                    TupleKey = new TupleKey()
                    {
                        Object = @object,
                        Relation = relation,
                        User = user
                    }
                });
                if (!result!.Allowed)
                    context.Fail(new AuthorizationFailureReason(this, "Sandcastle check was denied for {reason}"));
            }
            context.Succeed(requirement);
        }
    }
}


// Microsoft's recommendation, not a great time beyond simple scenarios
public class SandcastleAuthorizeAttribute : AuthorizeAttribute
{
    public SandcastleAuthorizeAttribute(string @object, string relation, string user)
    {
        Policy = new FgaCheck(@object, relation, user).ToString();
    }
}




[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SandcastleComputedAuthorizeAttribute : Attribute
{
    public SandcastleComputedAuthorizeAttribute(Func<HttpContext, string> @object, Func<HttpContext, string> relation, Func<HttpContext, string> user)
    {
        Object = @object;
        Relation = relation;
        User = user;
    }

    public Func<HttpContext, string> Object { get; }
    public Func<HttpContext, string> Relation { get; }
    public Func<HttpContext, string> User { get; }

}

public class RequireEntityAccessAttribute : SandcastleComputedAuthorizeAttribute
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

public class RequireSiteAccessAttribute : SandcastleComputedAuthorizeAttribute
{
    public RequireSiteAccessAttribute() 
        : base(context => $"site:{context.GetRouteValue("siteId")}", _ => "member", context => context.User.Identity!.Name!)
    {
    }
}

[Authorize("Sandcastle")]
[RequireSiteAccess]
[Route("api/{siteId}/sampling")]
public class SamplingController : ControllerBase
{

}


