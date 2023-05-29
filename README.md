## OpenFGA & Auth0 FGA for ASP.NET Core + Worker Services

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.DependencyInjection?label=Fga.Net.DependencyInjection&style=flat-square)](https://www.nuget.org/packages/Fga.Net.DependencyInjection)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.AspNetCore?label=Fga.Net.AspNetCore&style=flat-square)](https://www.nuget.org/packages/Fga.Net.AspNetCore)

#### Note: This project is currently in beta. Breaking changes may occur before release.

### Packages
**`Fga.Net.DependencyInjection`**: Provides dependency injection/configuration extensions for [OpenFga.Sdk](https://github.com/openfga/dotnet-sdk)

**`Fga.Net.AspNetCore`**: Authorization middleware to perform FGA checks for inbound requests.

## Getting Started

This package is compatible with the OSS OpenFGA as well as the managed Auth0 FGA service. Usage of DSL v1.1 is required.

Please ensure you have a basic understanding of how FGA works before continuing: [OpenFGA Docs](https://openfga.dev/) or [Auth0 FGA Docs](https://docs.fga.dev/)

## ASP.NET Core Setup

This tutorial assumes you have authentication setup within your project, such as [JWT bearer authentication via Auth0](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization).

Install `Fga.Net.AspNetCore` from Nuget before continuing.

### Auth0 FGA

Ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys).

1. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
2. Add the following code to your ASP.NET Core services configuration:
```cs
builder.Services.AddOpenFgaClient(config =>
{
    config.ConfigureAuth0Fga(x =>
    {
        x.WithAuthentication(builder.Configuration["Auth0Fga:ClientId"]!, builder.Configuration["Auth0Fga:ClientSecret"]!);
    });

    config.SetStoreId(builder.Configuration["Auth0Fga:StoreId"]!);
});

builder.Services.AddOpenFgaMiddleware();
```

The `ConfigureAuth0Fga` extension will configure the client to work with the Auth0 US environment. An environment selector will be added as additional regions come online.

### OpenFGA

1. Add the FGA `ApiScheme`, `ApiHost` & `StoreId` to your application configuration.
2. Add the following code to your ASP.NET Core configuration:
```cs
services.AddOpenFgaClient(config =>
{
    config.ConfigureOpenFga(x =>
    {
        x.SetConnection(context.Configuration["Fga:ApiScheme"] context.Configuration["Fga:ApiHost"]);
    });
    config.SetStoreId(context.Configuration["Fga:StoreId"]);
});

builder.Services.AddOpenFgaMiddleware();
```

Authentication can be added to OpenFGA connections via the relevant extensions:

```csharp
config.ConfigureOpenFga(x =>
{
    x.SetConnection(Uri.UriSchemeHttp, context.Configuration["Fga:ApiHost"]);
    
    // Add API key auth
    x.WithApiKeyAuthentication(context.Configuration["Fga:ApiKey"]);
    // or OIDC auth
    x.WithOidcAuthentication(
        context.Configuration["Fga:ClientId"], 
        context.Configuration["Fga:ClientSecret"], 
        context.Configuration["Fga:Issuer"], 
        context.Configuration["Fga:Audience"]);
});

```

### Authorization Policy Setup

Your authorization policy should be configured with `RequireAuthenticatedUser` and `AddFgaRequirement` at minimum:

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, 
        p => p
            .RequireAuthenticatedUser()
            .AddFgaRequirement());
});
```

A constant authorization key is included for convenience, but `AddFgaRequirement` can be used with any additional policy as required.

### Built-in Check Attributes

`Fga.Net.AspNetCore` ships with a number of attributes that should cover the most common authorization sources for FGA checks:

- `FgaHeaderObjectAttribute` - Computes the Object via a value in the requests header
- `FgaPropertyObjectAttribute` - Computes the Object via a root-level property on the requests JSON body
- `FgaQueryObjectAttribute` - Computes the Object via a value in the query string
- `FgaRouteObjectAttribute` - Computes the Object via a value in the routes path

If you want to use these attributes, you need to configure how the user's identifier is constructed from the users claims.
The example below uses the Name, which is mapped to the User ID in a default Auth0 integration.

```cs
builder.Services.AddOpenFgaMiddleware(config =>
{
    //'user' should be the name of the user type that you're using within your FGA model
    config.SetUserIdentifier("user", principal => principal.Identity!.Name!);
});
```

These attributes can then be used in both minimal APIs & in your controller(s):
```cs
    // Traditional Controllers
    [ApiController]
    [Route("[controller]")]
    [Authorize(FgaAuthorizationDefaults.PolicyKey)]
    public class DocumentController : ControllerBase
    {  
        [HttpGet("view/{documentId}")]
        [FgaRouteObject("read", "document", nameof(documentId))]
        public string GetByConvention(string documentId)
        {
            return documentId;
        }
    }

    // Minimal APIs
    app.MapGet("/viewminimal/{documentId}", (string documentId) => Task.FromResult(documentId))
        .RequireAuthorization(FgaAuthorizationDefaults.PolicyKey)
        // Extensions methods are included for the built-in attributes
        .WithFgaRouteCheck("read", "document", "documentId")
        // You can apply custom attributes like so
        .WithMetadata(new ComputedRelationshipAttribute("document", "documentId"));
```

### Custom Attributes

If your requirements are more bespoke than can be covered by the built-in attributes, then you may want to implement your own.
To do this, inherit from either `FgaBaseObjectAttribute`, which uses the configuration's user resolver, or from `FgaAttribute` which is the root attribute and permits you to implement a custom user source.

For example, an equivalent to the [How To Integrate Within A Framework](https://docs.fga.dev/integration/framework) tutorial would be:
```cs
public class ComputedRelationshipAttribute : FgaBaseObjectAttribute
{
    private readonly string _prefix;
    private readonly string _routeValue;
    
    public ComputedRelationshipAttribute(string prefix, string routeValue)
    {
        _prefix = prefix;
        _routeValue = routeValue;
    }
    
    public override ValueTask<string> GetRelation(HttpContext context) 
        => ValueTask.FromResult(context.Request.Method switch 
        {
            "GET" => "viewer",
            "POST" => "writer",
            _ => "owner"
        });

    public override ValueTask<string> GetObject(HttpContext context) 
        => ValueTask.FromResult(FormatObject(_type, context.GetRouteValue(_routeValue)!.ToString()!));
}
```

An additional pre-made attribute that allows all tuple values to be hardcoded strings ships with the package (`FgaStringAttribute`). This attribute is useful for testing and debug purposes, but should not be used in a real application.

## Client Injection

This package registers both the `OpenFgaApi` and `OpenFgaClient` types in the DI container. `OpenFgaClient` is a higher level abstraction and preferred over `OpenFgaApi` for general use.

## Testing

When running tests against your API or service collection, you likely want a different client configuration than usual. You can achieve this by calling `PostConfigureFgaClient` on your services configuration:

```cs
// Replaces existing configuration
services.PostConfigureFgaClient(config =>
{
    config.SetStoreId(storeId);
    config.ConfigureOpenFga(x =>
    {
        x.SetConnection(Uri.UriSchemeHttp, openFgaUrl);
    });
});

```

## Worker Service / Generic Host Setup

`Fga.Net.DependencyInjection` ships with the `AddOpenFgaClient` service collection extension that handles all required wire-up.

To get started:

1. Install `Fga.Net.DependencyInjection`
2. Add your `StoreId`, `ClientId` and `ClientSecret` Auth0 FGA configuration **OR** `ApiScheme`, `ApiHost` & `StoreId` OpenFGA configuration to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
3. Register the authorization client:

```cs
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
         // Auth0 FGA
        services.AddOpenFgaClient(config =>
        {
            config.ConfigureAuth0Fga(x =>
            {
                x.WithAuthentication(context.Configuration["Auth0Fga:ClientId"], context.Configuration["Auth0Fga:ClientSecret"]);
            });
            config.SetStoreId(context.Configuration["Auth0Fga:StoreId"]);
        });
        
        // OpenFGA
        services.AddOpenFgaClient(config =>
        {
            config.ConfigureOpenFga(x =>
            {
                x.SetConnection(Uri.UriSchemeHttp, context.Configuration["Fga:ApiHost"]);
                
                // Optionally add authentication settings
                x.WithApiKeyAuthentication(context.Configuration["Fga:ApiKey"]);
                x.WithOidcAuthentication(
                    context.Configuration["Fga:ClientId"], 
                    context.Configuration["Fga:ClientSecret"], 
                    context.Configuration["Fga:Issuer"], 
                    context.Configuration["Fga:Audience"]);
            });
            config.SetStoreId(context.Configuration["Fga:StoreId"]);
        });

        services.AddHostedService<MyBackgroundWorker>();
    })
    .Build();

await host.RunAsync();
```

4. Request the client in your services:

```cs
public class MyBackgroundWorker : BackgroundService
{
    private readonly OpenFgaClient _fgaClient;

    public MyBackgroundWorker(OpenFgaClient fgaClient)
    {
        _fgaClient = fgaClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Do work with the client
    }
}
```

## Standalone client setup

See the [OpenFGA.Sdk docs](https://openfga.dev/docs/getting-started/setup-sdk-client)

## Disclaimer

I am not affiliated with nor represent Auth0 or OpenFGA. All support queries regarding the underlying service should go to the [Auth0 Labs Discord](https://discord.gg/8naAwJfWN6).
