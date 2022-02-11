# Auth0 FGA for .NET & ASP.NET Core

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net?label=Fga.Net&style=flat-square)](https://www.nuget.org/packages/Fga.Net)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net?label=Fga.Net.AspNetCore&style=flat-square)](https://www.nuget.org/packages/Fga.Net.AspNetCore)

Please ensure you have a basic understanding of how FGA works before continuing: https://docs.fga.dev/

#### Note: This project is in its early stages and will have breaking changes as FGA matures.

## ASP.NET Core Setup

Before getting started, ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys).

I'm also assuming you have authentication setup within your project, such as [JWT bearer authentication via Auth0](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization).


1. Install `Fga.Net.AspNetCore` from Nuget.
2. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
3. Add the following code to your ASP.NET Core configuration:
```cs
// Registers FgaAuthenticationClient, FgaAuthorizationClient, and the authorization handler
builder.Services.AddAuth0Fga(x =>
{
    x.ClientId = builder.Configuration["Auth0Fga:ClientId"];
    x.ClientSecret = builder.Configuration["Auth0Fga:ClientSecret"];
});

// Register the authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, 
        p => p
            .RequireAuthenticatedUser()
            .AddFgaRequirement(builder.Configuration["Auth0Fga:StoreId"]));
});
```

4. Create an attribute that inherits from `TupleCheckAttribute`. From here, you can pull metadata you require to perform your tuple checks out of the HTTP request.
For example, an equivalent to the [How To Integrate Within A Framework](https://docs.fga.dev/integration/framework) example would be:
```cs
public class EntityAuthorizationAttribute : TupleCheckAttribute
{
    private readonly string _prefix;
    private readonly string _routeValue;
    public EntityAuthorizationAttribute(string prefix, string routeValue)
    {
        _prefix = prefix;
        _routeValue = routeValue;
    }

    public override ValueTask<string> GetUser(HttpContext context) 
        => ValueTask.FromResult(context.User.Identity!.Name!);

    public override ValueTask<string> GetRelation(HttpContext context) 
        => ValueTask.FromResult(context.Request.Method switch 
        {
            "GET" => "viewer",
            "POST" => "writer",
            _ => "owner"
        });

    public override ValueTask<string> GetObject(HttpContext context) 
        => ValueTask.FromResult($"{_prefix}:{context.GetRouteValue(_routeValue)}");
}
```

5. Apply the `Authorize` and `EntityAuthorization` attributes to your controller(s):
```cs
    [ApiController]
    [Route("[controller]")]
    [Authorize(FgaAuthorizationDefaults.PolicyKey)]
    public class DocumentController : ControllerBase
    {  
        [HttpGet("view/{documentId}")]
        [EntityAuthorization("doc", "documentId")]
        public string GetByConvention(string documentId)
        {
            return documentId;
        }
    }
```

If you need to manually perform checks, inject the `IFgaAuthorizationClient` as required.

An additional pre-made attribute that allows all tuple values to be hardcoded strings ships with the package (`StringTupleCheckAttribute`). This attrbute is useful for testing and debug purposes, but should not be used in a real application.

## Worker Service / Generic Host setup
Full docs coming soon.

`Fga.Net` ships with the `AddAuth0FgaAuthenticationClient` and `AddAuth0FgaAuthorizationClient` service collection extensions that should be self-explanatory. To use the authorization client, both clients must be registered.

## Standalone client setup

Useful for testing. 
I would not recommend a standalone client setup outside of lambda scenarios as the `HttpClient` lifetime is not automatically maintained.


1. Install `Fga.Net`
2. Create the authorization client as below:
```cs
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
```

## Internal Cache

The `FgaTokenCache` will cache the FGA authorization token until 15 minutes before expiry. This is not currently customizable.

This cache is automatically enabled if you use any of the DI extensions, as well as `FgaAuthorizationClient.Create`.

## Disclaimer

I am not affiliated with nor represent Auth0. All support queries regarding the underlying service should go to the [Auth0 Labs Discord](https://discord.gg/8naAwJfWN6).
