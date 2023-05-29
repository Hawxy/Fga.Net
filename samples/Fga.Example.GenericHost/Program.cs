using Fga.Example.GenericHost;
using Fga.Net.DependencyInjection;

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