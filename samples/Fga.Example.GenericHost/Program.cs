using Fga.Example.GenericHost;
using Fga.Net.DependencyInjection;
using Fga.Net.DependencyInjection.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOpenFgaClient(config =>
        {
            // Auth0 FGA
            config.ConfigureAuth0Fga(x =>
            {
                x.WithAuthentication(context.Configuration["Auth0Fga:ClientId"], context.Configuration["Auth0Fga:ClientSecret"]);
            });
            config.SetStoreId(context.Configuration["Auth0Fga:StoreId"]);

            // OpenFGA
            config.ConfigureOpenFga(x =>
            {
                x.SetConnection(Uri.UriSchemeHttp, context.Configuration["Fga:ApiHost"]);
            });
            config.SetStoreId(context.Configuration["Fga:StoreId"]);
        });

        services.AddHostedService<MyBackgroundWorker>();
    })
    .Build();

await host.RunAsync();