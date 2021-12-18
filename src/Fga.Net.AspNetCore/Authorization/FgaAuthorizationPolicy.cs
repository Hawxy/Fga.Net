using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization;


public class SandcastleRequirement : IAuthorizationRequirement
{
}
// Think about if this should be a 1:1 handler, A IAuthorizationHandler or a requirement that implements its own handler.
public class SandcastleAuthorizationHandler : AuthorizationHandler<SandcastleRequirement>
{
    private readonly FgaAuthorizationClient _client;

    public SandcastleAuthorizationHandler(FgaAuthorizationClient client)
    {
        _client = client;
    }


    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SandcastleRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is null)
                return;
            var attributes = endpoint!.Metadata.GetOrderedMetadata<ComputedAuthorizationAttribute>();
            // The user is enforcing the sandcastle policy but there's no attributes here, bit odd.
            if (attributes.Count == 0)
                return;
            foreach (var attribute in attributes)
            {
                var user = await attribute.GetUser(httpContext);
                var relation = await attribute.GetRelation(httpContext);
                var @object = await attribute.GetObject(httpContext);

                var result = await _client.CheckAsync(new CheckRequest
                {
                    TupleKey = new TupleKey
                    {
                        User = user,
                        Relation = relation,
                        Object = @object
                    }
                });
                if (result is null || !result.Allowed)
                    context.Fail(new AuthorizationFailureReason(this, "Sandcastle check was denied for {reason}"));
            }
            context.Succeed(requirement);
        }
    }
}


