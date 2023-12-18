#region License
/*
   Copyright 2021-2024 Hawxy (JT)

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
using Fga.Net.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Computes a FGA Authorization check based on value within the route.
/// </summary>
public class FgaRouteObjectAttribute : FgaBaseObjectAttribute
{
    private readonly string _relation;
    private readonly string _type;
    private readonly string _routeKey;

    /// <summary>
    /// Computes a FGA Authorization check based on value within the route.
    /// </summary>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="routeKey">The route key to get the value from. Will throw an exception if not present.</param>
    public FgaRouteObjectAttribute(string relation, string type, string routeKey)
    {
        _relation = relation;
        _type = type;
        _routeKey = routeKey;
    }

    /// <inheritdoc />
    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult(_relation);
    }
    /// <inheritdoc />
    public override ValueTask<string> GetObject(HttpContext context)
    {
        var routeValue = context.GetRouteValue(_routeKey);

        if (routeValue is not null)
        {
            return ValueTask.FromResult(FormatObject(_type, routeValue.ToString()!));
        }

        throw new FgaMiddlewareException($"Route value {_routeKey} was not present in the request");
    }
}