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
/// An authorization attribute that provides metadata from the current HTTP request.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class ComputedAuthorizationAttribute : Attribute
{
    /// <summary>
    /// An entity in the system that can be related to an object.
    /// </summary>
    /// <param name="context">The context of the current request</param>
    /// <returns>A user identifier</returns>
    public abstract ValueTask<string> GetUser(HttpContext context);
    /// <summary>
    /// Defines the possibility of a relationship between objects of this type and other users in the system.
    /// </summary>
    /// <param name="context">The context of the current request</param>
    /// <returns>A relationship, such as reader or writer</returns>
    public abstract ValueTask<string> GetRelation(HttpContext context);
    /// <summary>
    /// An entity in the system. 
    /// </summary>
    /// <param name="context">The context of the current request</param>
    /// <returns>Usually a string in an entity-identifier format: <value>document:id</value></returns>
    public abstract ValueTask<string> GetObject(HttpContext context);
}

