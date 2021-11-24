using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sandcastle.AspNetCore.Controllers;

public class SandcastleControllerBase : ControllerBase
{
    private readonly SandcastleAuthorizationClient _client;
    public SandcastleControllerBase(SandcastleAuthorizationClient client)
    {
        _client = client;
    }

    public async Task<bool> Check(string @object, string relation, string user)
    {
        var checkRes = await _client.CheckAsync(new CheckRequest
        {
            AuthorizationModelId = "",
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