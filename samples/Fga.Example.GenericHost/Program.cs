using Fga.Example.GenericHost;
using Fga.Net;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAuth0FgaAuthenticationClient();
        services.AddAuth0FgaAuthorizationClient(config =>
        {
            config.ClientId = context.Configuration["Auth0Fga:ClientId"];
            config.ClientSecret = context.Configuration["Auth0Fga:ClientSecret"];
        });

        services.AddHostedService<MyBackgroundWorker>();
    })
    .Build();

await host.RunAsync();