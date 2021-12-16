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
            var attributes = endpoint!.Metadata.GetOrderedMetadata<ComputedAuthorizeAttribute>();
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


