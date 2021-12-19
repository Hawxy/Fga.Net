# Auth0 FGA for .NET & ASP.NET Core

Please ensure you have a basic understanding of how FGA works before continuing: https://docs.fga.dev/

#### Note: This project is an early alpha and is subject to breaking changes without notice.

### ASP.NET Core Setup

Before getting started, ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys)

1. Install `Fga.Net.AspNetCore` from Nuget
2. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage)
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


### Worker Service setup
Coming soon!

### Standalone client setup
Coming soon!

