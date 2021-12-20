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

