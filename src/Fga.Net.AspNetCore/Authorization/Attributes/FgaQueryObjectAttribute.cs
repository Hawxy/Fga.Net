#region License
/*
   Copyright 2021-2025 Hawxy (JT)

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

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Computes a FGA Authorization check based on a query value
/// </summary>
public class FgaQueryObjectAttribute : FgaBaseObjectAttribute
{
    private readonly string _relation;
    private readonly string _type;
    private readonly string _queryKey;

    /// <summary>
    /// Computes a FGA Authorization check based on a query value
    /// </summary>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="queryKey">The query key to get the value from. Will throw an exception if not present.</param>
    public FgaQueryObjectAttribute(string relation, string type, string queryKey)
    {
        _relation = relation;
        _type = type;
        _queryKey = queryKey;
    }


    /// <inheritdoc />
    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult(_relation);
    }

    /// <inheritdoc />
    public override ValueTask<string> GetObject(HttpContext context)
    {
        if (context.Request.Query.TryGetValue(_queryKey, out var queryValue))
        {
            return ValueTask.FromResult(FormatObject(_type, queryValue!));
        }

        throw new FgaMiddlewareException($"Query key {_queryKey} was not present in the request");
    }
}