using System.Security.Claims;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });


// Auth0 FGA
/*builder.Services.AddOpenFgaClient(clientConfig =>
{
    clientConfig.WithAuth0FgaDefaults(builder.Configuration["Auth0Fga:ClientId"]!,
        builder.Configuration["Auth0Fga:ClientSecret"]!);
    clientConfig.StoreId = builder.Configuration["Auth0Fga:StoreId"];

});*/

builder.Services.AddOpenFgaClient(config =>
{
    config.ConfigureAuth0Fga(x =>
    {
        x.WithAuthentication(builder.Configuration["Auth0Fga:ClientId"]!, builder.Configuration["Auth0Fga:ClientSecret"]!);
    });

    config.SetStoreId(builder.Configuration["Auth0Fga:StoreId"]!);
});

// OpenFGA
builder.Services.AddOpenFgaClient(x =>
{
    x.SetStoreId(builder.Configuration["Fga:StoreId"]);
    
    x.ApiScheme = builder.Configuration["Fga:ApiScheme"];
    x.ApiHost = builder.Configuration["Fga:ApiHost"];
    x.StoreId = builder.Configuration["Fga:StoreId"];
});

builder.Services.AddOpenFgaMiddleware(middlewareConfig =>
{
    middlewareConfig.UserIdentityResolver = principal => $"user:{principal.Identity!.Name!}";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(FgaAuthorizationDefaults.PolicyKey, 
        p => p
            .RequireAuthenticatedUser()
            .AddFgaRequirement());
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/viewminimal/{documentId}", (string documentId) => Task.FromResult(documentId))
    .RequireAuthorization(FgaAuthorizationDefaults.PolicyKey)
    .WithFgaRouteCheck("reader", "document", "documentId");

app.Run();
