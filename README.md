# Auth0 FGA for .NET & ASP.NET Core

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net?label=Fga.Net&style=flat-square)](https://www.nuget.org/packages/Fga.Net)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net?label=Fga.Net.AspNetCore&style=flat-square)](https://www.nuget.org/packages/Fga.Net.AspNetCore)

Please ensure you have a basic understanding of how FGA works before continuing: https://docs.fga.dev/

#### Note: This project is an early alpha and is subject to breaking changes without notice.

## ASP.NET Core Setup

Before getting started, ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys).

I'm also assuming you have authentication setup within your project, such as [JWT bearer authentication via Auth0](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization).


1. Install `Fga.Net.AspNetCore` from Nuget.
2. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
3. Add the following code to your ASP.NET Core configuration:
```cs
// Registers FgaAuthenticationClient & FgaAuthorizationClient, and the authorization handler
builder.Services.AddAuth0Fga(x =>
{
    x.ClientId = builder.Configuration["Auth0Fga:ClientId"];
    x.ClientSecret = builder.Configuration["Auth0Fga:ClientSecret"];
    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
});

// Register the authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, p => p.RequireAuthenticatedUser().AddFgaRequirement());
});
```

4. Create an authorization attribute that inherits from `ComputedAuthorizationAttribute`. From here, you can pull metadata you require to perform your tuple checks out of the HTTP request.
For example, an equivalent to the [How To Integrate Within A Framework](https://docs.fga.dev/integration/framework) example would be:
```cs
public class EntityAuthorizationAttribute : ComputedAuthorizationAttribute
{
    private readonly string _prefix;
    private readonly string _routeValue;
    public EntityAuthorizationAttribute(string prefix, string routeValue)
    {
        _prefix = prefix;
        _routeValue = routeValue;
    }

    public override ValueTask<string> GetUser(HttpContext context)
    {
        return ValueTask.FromResult(context.User.Identity!.Name!);
    }

    public override ValueTask<string> GetRelation(HttpContext context)
    {
        return ValueTask.FromResult(context.Request.Method switch
        {
            "GET" => "viewer",
            "POST" => "writer",
            _ => "owner"
        });
    }

    public override ValueTask<string> GetObject(HttpContext context)
    {
        return ValueTask.FromResult($"{_prefix}:{context.GetRouteValue(_routeValue)}");
    }
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

If you need to manually perform checks, inject the `FgaAuthorizationClient` as required.

An additional pre-made attribute that allows all tuple values to be hardcoded strings ships with the package (`StringComputedAuthorizationAttribute`). This attrbute is useful for testing and debug purposes, but should not be used in a real application.

## Worker Service / Generic Host setup
Full docs coming soon.

`Fga.Net` ships with the `AddAuth0FgaAuthenticationClient` and `AddAuth0FgaAuthorizationClient` service collection extensions that should be self-explanatory. To use the authorization client, both clients must be registered.

## Standalone client setup

Seriously consider if you need to run a standalone client before picking this option.

1. Install `Fga.Net`
2. Create the authorization client as below:
```cs
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
```

