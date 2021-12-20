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

using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// A simple implementation of <see cref="ComputedAuthorizationAttribute"/> that only accepts strings.
/// Useful for debugging and testing purposes. Do not use in a real application
/// </summary>
public class StringComputedAuthorizationAttribute : ComputedAuthorizationAttribute
{
    private readonly string _user;
    private readonly string _relation;
    private readonly string _object;
    /// <summary>
    /// Constructs a new <see cref="StringComputedAuthorizationAttribute"/>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="relation"></param>
    /// <param name="object"></param>
    public StringComputedAuthorizationAttribute(string user, string relation, string @object)
    {
        _user = user;
        _relation = relation;
        _object = @object;
    }
    /// <inheritdoc />
    public override ValueTask<string> GetUser(HttpContext context) => ValueTask.FromResult(_user);
    /// <inheritdoc/>
    public override ValueTask<string> GetRelation(HttpContext context) => ValueTask.FromResult(_relation);
    /// <inheritdoc />
    public override ValueTask<string> GetObject(HttpContext context) => ValueTask.FromResult(_object);
}