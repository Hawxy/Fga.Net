using Fga.Net.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Net.AspNetCore.Controllers;

/// <summary>
/// A base ASP.NET Core controller that adds an extension method for FGA checks.
/// </summary>
public class FgaControllerBase : ControllerBase
{
    private readonly IFgaAuthorizationClient _client;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    public FgaControllerBase(IFgaAuthorizationClient client)
    {
        _client = client;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="relation"></param>
    /// <param name="object"></param>
    /// <returns></returns>
    public async Task<bool> Check(string user, string relation, string @object)
    {
        var checkRes = await _client.CheckAsync(new CheckTupleRequest
        {
            TupleKey = new TupleKey
            {
                User = user,
                Relation = relation,
                Object = @object
            }
        });
        return checkRes!.Allowed;
    }
}