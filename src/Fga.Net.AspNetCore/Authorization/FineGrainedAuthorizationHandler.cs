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

using Auth0.Fga.Api;
using Auth0.Fga.Model;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization;

internal class FineGrainedAuthorizationHandler : AuthorizationHandler<FineGrainedAuthorizationRequirement>
{
    private readonly Auth0FgaApi _client;

    public FineGrainedAuthorizationHandler(Auth0FgaApi client)
    {
        _client = client;
    }


    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FineGrainedAuthorizationRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is null)
                return;
            var attributes = endpoint.Metadata.GetOrderedMetadata<TupleCheckAttribute>();
            // The user is enforcing the fga policy but there's no attributes here.
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
                
                var result = await _client.Check(new CheckRequest()
                {
                    TupleKey = new TupleKey
                    {
                        User = user,
                        Relation = relation,
                        Object = @object
                    }
                }, httpContext.RequestAborted);


                results.Add(result.Allowed);
            }

            if(results.All(x => x))
                context.Succeed(requirement);
        }
    }
}


