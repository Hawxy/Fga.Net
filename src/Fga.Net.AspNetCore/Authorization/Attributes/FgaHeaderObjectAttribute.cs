using Fga.Net.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// Computes a FGA Authorization check based on the header value of the request
/// </summary>
public class FgaHeaderObjectAttribute : FgaBaseObjectAttribute
{
    private readonly string _relation;
    private readonly string _type;
    private readonly string _headerKey;

    /// <summary>
    /// Computes a FGA Authorization check based on the header value of the request
    /// </summary>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="headerKey">The header to get the value from. Will throw an exception if not present.</param>
    public FgaHeaderObjectAttribute(string relation, string type, string headerKey)
    {
        _relation = relation;
        _type = type;
        _headerKey = headerKey;
    }

    /// <inheritdoc />
    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult(_relation);
    }

    /// <inheritdoc />
    public override ValueTask<string> GetObject(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_headerKey, out var header))
        {
            return ValueTask.FromResult(FormatObject(_type, header.ToString()));
        }

        throw new FgaMiddlewareException($"Header {_headerKey} was not present in the request");
    }
}