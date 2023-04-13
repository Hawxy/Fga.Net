## OpenFGA & Auth0 FGA for ASP.NET Core + Worker Services

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.DependencyInjection?label=Fga.Net.DependencyInjection&style=flat-square)](https://www.nuget.org/packages/Fga.Net.DependencyInjection)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.AspNetCore?label=Fga.Net.AspNetCore&style=flat-square)](https://www.nuget.org/packages/Fga.Net.AspNetCore)

#### Note: This project is in its early stages and will have breaking changes as FGA matures.

### Packages
**`Fga.Net.DependencyInjection`**: Provides dependency injection/configuration extensions for [OpenFga.Sdk](https://github.com/openfga/dotnet-sdk)

**`Fga.Net.AspNetCore`**: Additionally includes Authorization middleware to support FGA checks as part of a request's lifecycle.

## Getting Started

This package is compatible with the OSS OpenFGA as well as the managed Auth0 FGA service.

Please ensure you have a basic understanding of how FGA works before continuing: [OpenFGA Docs](https://openfga.dev/) or [Auth0 FGA Docs](https://docs.fga.dev/)

## ASP.NET Core Setup

This tutorial assumes you have authentication setup within your project, such as [JWT bearer authentication via Auth0](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization).

Install `Fga.Net.AspNetCore` from Nuget before continuing.

### Auth0 FGA

Ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys).


1. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
2. Add the following code to your ASP.NET Core services configuration:
```cs
builder.Services.AddOpenFgaClient(x =>
{
    x.WithAuth0FgaDefaults(builder.Configuration["Auth0Fga:ClientId"], builder.Configuration["Auth0Fga:ClientSecret"]);

    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
});

builder.Services.AddOpenFgaMiddleware();
```

The `WithAuth0FgaDefaults` extension will configure the relevant OpenFGA client settings to work with Auth0 FGA's US environment.

### OpenFGA

OpenFGA configuration is very similar to the [SDK Setup Guide](https://openfga.dev/docs/getting-started/setup-sdk-client)

1. Add the FGA `ApiScheme`, `ApiHost` & `StoreId` to your application configuration.
2. Add the following code to your ASP.NET Core configuration:
```cs
builder.Services.AddOpenFgaClient(x =>
{
    x.ApiScheme = builder.Configuration["Fga:ApiScheme"];
    x.ApiHost = builder.Configuration["Fga:ApiHost"];
    x.StoreId = builder.Configuration["Fga:StoreId"];
});

builder.Services.AddOpenFgaMiddleware();
```

### Authorization Policy Setup

We'll need to setup our authorization policy like so:

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, 
        p => p
            .RequireAuthenticatedUser()
            .AddFgaRequirement());
});
```

### Built-in Attributes

`Fga.Net.AspNetCore` ships with a number of attributes that should cover the most common authorization sources for FGA checks:

- `FgaHeaderObjectAttribute` - Computes the Object via a value in the requests header
- `FgaPropertyObjectAttribute` - Computes the Object via a root-level property on the requests JSON body
- `FgaQueryObjectAttribute` - Computes the Object via a value in the query string
- `FgaRouteObjectAttribute` - Computes the Object via a value in the routes path

These attributes can be used in both minimal APIs & in your controller(s):
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
        .WithMetadata(new FgaRouteObjectAttribute("read", "document", "documentId"));
```


If you want to use the built-in attributes, you need to configure how the user's identity is resolved from the `ClaimsPrincipal`.
The example below uses the Name, which should be suitable for most people (given the claim is mapped correctly).

```cs
builder.Services.AddOpenFgaMiddleware(config =>
{
    //DSL v1.1 requires the user type to be included
    config.UserIdentityResolver = principal => $"user:{principal.Identity!.Name!}";
});
```

### Custom Attributes

If your requirements are more bespoke than can be covered by the built-in attributes, then you may want to implement your own.
To do this, inherit from either `FgaBaseObjectAttribute`, which uses the configuration's user resolver, or from `FgaAttribute` which is the root attribute and permits you to implement a custom user source.

For example, an equivalent to the [How To Integrate Within A Framework](https://docs.fga.dev/integration/framework) tutorial would be:
```cs
public class ComputedRelationshipAttribute : FgaAttribute
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
        => ValueTask.FromResult(FormatObject(_type, context.GetRouteValue(_routeValue)!.ToString()!));
}
```

If you need to manually perform checks, inject the `Auth0FgaApi` as required.

An additional pre-made attribute that allows all tuple values to be hardcoded strings ships with the package (`FgaStringAttribute`). This attribute is useful for testing and debug purposes, but should not be used in a real application.

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
        services.AddOpenFgaClient(config =>
        {
            // Auth0 FGA
            config.WithAuth0FgaDefaults(context.Configuration["Auth0Fga:ClientId"], context.Configuration["Auth0Fga:ClientSecret"]);
            config.StoreId = context.Configuration["Auth0Fga:StoreId"];

            // OpenFGA
            config.ApiScheme = context.Configuration["Fga:ApiScheme"];
            config.ApiHost = context.Configuration["Fga:ApiHost"];
            config.StoreId = context.Configuration["Fga:StoreId"];
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
    private readonly OpenFgaApi _authorizationClient;

    public MyBackgroundWorker(OpenFgaApi authorizationClient)
    {
        _authorizationClient = authorizationClient;
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
