using Fga.Net;
using Fga.Net.Authentication;
using Fga.Net.Authorization;

var client = FgaAuthorizationClient.Create(FgaAuthenticationClient.Create(), new FgaClientConfiguration()
{
    ClientId = args[0],
    ClientSecret = args[1],
    StoreId = args[2]
});

var response = await client.CheckAsync(new CheckTupleRequest()
{
    TupleKey = new TupleKey()
    {
        User = "",
        Relation = "",
        Object = ""
    }
});