using Fga.Net.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fga.Net.AspNetCore.Controllers;

public class FgaControllerBase : ControllerBase
{
    private readonly FgaAuthorizationClient _client;
    public FgaControllerBase(FgaAuthorizationClient client)
    {
        _client = client;
    }

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