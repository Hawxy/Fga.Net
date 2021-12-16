using Microsoft.AspNetCore.Mvc;

namespace Fga.Net.AspNetCore.Controllers;

public class FgaControllerBase : ControllerBase
{
    private readonly FgaAuthorizationClient _client;
    public FgaControllerBase(FgaAuthorizationClient client)
    {
        _client = client;
    }

    public async Task<bool> Check(string @object, string relation, string user)
    {
        var checkRes = await _client.CheckAsync(new CheckRequest
        {
            TupleKey = new TupleKey
            {
                Object = @object,
                Relation = relation,
                User = user
            }
        });
        return checkRes!.Allowed;
    }
}