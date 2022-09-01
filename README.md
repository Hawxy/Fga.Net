# OpenFGA & Auth0 FGA for ASP.NET Core/Worker Services

#### Note: This project is in its early stages and will have breaking changes as FGA matures.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.DependencyInjection?label=Fga.Net.DependencyInjection&style=flat-square)](https://www.nuget.org/packages/Fga.Net.DependencyInjection)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fga.Net.AspNetCore?label=Fga.Net.AspNetCore&style=flat-square)](https://www.nuget.org/packages/Fga.Net.AspNetCore)

### Packages
- **Fga.Net.DependencyInjection**: Provides dependency injection/configuration extensions for OpenFga.Sdk

- **Fga.Net.AspNetCore**: Additionally includes Authorization middleware to support FGA checks as part of a request's lifecycle.

## Getting Started

This package is compatible with the open source OpenFGA implementation as well as the managed Auth0 FGA service.

Please ensure you have a basic understanding of how FGA works before continuing: [OpenFGA Docs](https://openfga.dev/) or [Auth0 FGA Docs](https://docs.fga.dev/)

## ASP.NET Core Setup

This tutorial assumes you have authentication setup within your project, such as [JWT bearer authentication via Auth0](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization).

Install `Fga.Net.AspNetCore` from Nuget before continuing.

### Auth0 FGA

Ensure you have a Store ID, Client ID, and Client Secret ready from [How to get your API keys](https://docs.fga.dev/integration/getting-your-api-keys).


1. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
2. Add the following code to your ASP.NET Core services configuration:
```cs
builder.Services.AddOpenFga(x =>
{
    x.WithAuth0FgaDefaults(builder.Configuration["Auth0Fga:ClientId"], builder.Configuration["Auth0Fga:ClientSecret"]);

    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
});
```

The `WithAuth0FgaDefaults` extension will configure the relevant OpenFGA client settings to work with Auth0 FGA's environment.

### OpenFGA

OpenFGA configuration is very similar to the [SDK Setup Guide](https://openfga.dev/docs/getting-started/setup-sdk-client)

1. Add the FGA `ApiScheme`, `ApiHost` & `StoreId` to your application configuration.
2. Add the following code to your ASP.NET Core configuration:
```cs
// Registers the OpenFgaApi client
builder.Services.AddOpenFga(x =>
{
    x.ApiScheme = builder.Configuration["Fga:ApiScheme"];
    x.ApiHost = builder.Configuration["Fga:ApiHost"];
    x.StoreId = builder.Configuration["Fga:StoreId"];
});
```

### Authorization Policy Setup

Now we'll need to setup our authorization middleware like so:

```cs
// Register the authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, 
        p => p
            .RequireAuthenticatedUser()
            .AddFgaRequirement());
});
```

Next, create an attribute that inherits from `TupleCheckAttribute`. From here, you can pull the metadata you require to perform your tuple checks out of the HTTP request.
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

Now apply the `Authorize` and `EntityAuthorization` attributes to your controller(s):
```cs
    // Traditional Controllers
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

    // Minimal APIs
    app.MapGet("/viewminimal/{documentId}",
    [Authorize(FgaAuthorizationDefaults.PolicyKey)] 
    [EntityAuthorization("doc", "documentId")]
    (documentId) => Task.FromResult(documentId));
```

If you need to manually perform checks, inject the `Auth0FgaApi` as required.

An additional pre-made attribute that allows all tuple values to be hardcoded strings ships with the package (`StringTupleCheckAttribute`). This attribute is useful for testing and debug purposes, but should not be used in a real application.

## Worker Service / Generic Host Setup

`Fga.Net.DependencyInjection` ships with the `AddAuth0FgaClient` service collection extension that handles all required wire-up.

To get started:

1. Install `Fga.Net.DependencyInjection`
2. Add your `StoreId`, `ClientId` and `ClientSecret` to your application configuration, ideally via the [dotnet secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
3. Register the authorization client:

```cs
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAuth0FgaClient(config =>
        {
            config.ClientId = context.Configuration["Auth0Fga:ClientId"];
            config.ClientSecret = context.Configuration["Auth0Fga:ClientSecret"];
            config.StoreId = context.Configuration["Auth0Fga:StoreId"];
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
    private readonly Auth0FgaApi _authorizationClient;

    public MyBackgroundWorker(Auth0FgaApi authorizationClient)
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

See the [Auth0.Fga docs](https://github.com/auth0-lab/fga-dotnet-sdk)

## Disclaimer

I am not affiliated with nor represent Auth0. All support queries regarding the underlying service should go to the [Auth0 Labs Discord](https://discord.gg/8naAwJfWN6).
