using Fga.Example.GenericHost;
using Fga.Net;
using Fga.Net.DependencyInjection;

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