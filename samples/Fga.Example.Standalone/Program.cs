using Fga.Net;
using Fga.Net.Authentication;
using Fga.Net.Authorization;


var clientId = args[0];
var clientSecret = args[1];
var storeId = args[2];

var client = FgaAuthorizationClient.Create(FgaAuthenticationClient.Create(), new FgaClientConfiguration
{
    ClientId = clientId,
    ClientSecret = clientSecret
});

var response = await client.CheckAsync(storeId, new CheckRequestParams
{
    Tuple_key = new TupleKey()
    {
        User = "",
        Relation = "",
        Object = ""
    }
});