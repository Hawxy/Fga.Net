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
using System.Text.Json;
using Fga.Net.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Computes a FGA Authorization check based on a root-level property within the object.
/// WARNING: This will result in double deserialization of the object and should not be used with large payloads.
/// </summary>
public class FgaPropertyObjectAttribute : FgaBaseObjectAttribute
{
    private readonly string _relation;
    private readonly string _type;
    private readonly string _property;

    /// <summary>
    /// Computes a FGA Authorization check based on a root-level property within the object.
    /// WARNING: This will result in double deserialization of the object and should not be used with large payloads.
    /// </summary>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="property">The JSON property to get the value from. Must be a string or number. Will throw an exception if not present.</param>
    public FgaPropertyObjectAttribute(string relation, string type, string property)
    {
        _relation = relation;
        _type = type;
        _property = property;
    }

    /// <inheritdoc />
    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult(_relation);
    }

    /// <inheritdoc />
    public override async ValueTask<string> GetObject(HttpContext context)
    {
       context.Request.EnableBuffering();
       using var document = await JsonDocument.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted);
       if (document.RootElement.TryGetProperty(_property, out var element))
       {
           context.Request.Body.Position = 0;
           return FormatObject(_type,element.GetString()!);
       }

       throw new FgaMiddlewareException($"Unable to resolve JSON property {_property}");
    }
}