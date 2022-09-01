using Fga.Example.GenericHost;
using Fga.Net.DependencyInjection;

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