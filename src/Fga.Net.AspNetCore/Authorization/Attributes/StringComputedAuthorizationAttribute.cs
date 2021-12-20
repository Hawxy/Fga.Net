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