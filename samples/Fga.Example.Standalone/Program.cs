using Fga.Net;
using Fga.Net.Authentication;
using Fga.Net.Authorization;


var client = FgaAuthorizationClient.Create(FgaAuthenticationClient.Create(), new FgaClientConfiguration()
{
    ClientId = args[0],
    ClientSecret = args[1]
});

var response = await client.CheckAsync(args[2], new CheckRequestParams()
{
    Tuple_key = new TupleKey()
    {
        User = "",
        Relation = "",
        Object = ""
    }
});