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

builder.Services.AddHttpLogging(x => { });

// Auth0 FGA
builder.Services.AddOpenFgaClient(config =>
{
    config.ConfigureAuth0Fga(x =>
    {
        x.WithAuthentication(builder.Configuration["Auth0Fga:ClientId"]!, builder.Configuration["Auth0Fga:ClientSecret"]!);
    });

    config.SetStoreId(builder.Configuration["Auth0Fga:StoreId"]!);
});

/* OpenFGA
builder.Services.AddOpenFgaClient(x =>
{
    x.ConfigureOpenFga(x =>
    {
        x.SetConnection(builder.Configuration["Fga:ApiScheme"]!, builder.Configuration["Fga:ApiHost"]!);
    });
    
    x.SetStoreId(builder.Configuration["Fga:StoreId"]);
});*/

builder.Services.AddOpenFgaMiddleware(middlewareConfig =>
{
    middlewareConfig.SetUserIdentifier("user", principal => principal.Identity!.Name!);
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
