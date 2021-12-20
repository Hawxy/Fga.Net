#region License
/*
   Copyright 2021-2022 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

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


