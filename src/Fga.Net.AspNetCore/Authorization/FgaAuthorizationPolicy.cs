using Fga.Net.AspNetCore.Authorization.Attributes;
using Fga.Net.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization;


internal class SandcastleRequirement : IAuthorizationRequirement
{
}
// Think about if this should be a 1:1 handler, A IAuthorizationHandler or a requirement that implements its own handler.
internal class SandcastleAuthorizationHandler : AuthorizationHandler<SandcastleRequirement>
{
    private readonly IFgaAuthorizationClient _client;

    public SandcastleAuthorizationHandler(IFgaAuthorizationClient client)
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
            var attributes = endpoint.Metadata.GetOrderedMetadata<ComputedAuthorizationAttribute>();
            // The user is enforcing the sandcastle policy but there's no attributes here, pass through.
            if (attributes.Count == 0)
                return;
            var results = new List<bool>();
            foreach (var attribute in attributes)
            {
                var user = await attribute.GetUser(httpContext);
                var relation = await attribute.GetRelation(httpContext);
                var @object = await attribute.GetObject(httpContext);
                // If we get back nulls from anything we cannot perform a check.
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(relation) || string.IsNullOrEmpty(@object))
                    return;
                
                var result = await _client.CheckAsync(new CheckTupleRequest
                {
                    TupleKey = new TupleKey
                    {
                        User = user,
                        Relation = relation,
                        Object = @object
                    }
                });
                //Something has gone wrong, short-circuit
                if (result is null)
                    return;
                results.Add(result.Allowed);
            }

            if(results.All(x=> x))
                context.Succeed(requirement);
        }
    }
}


