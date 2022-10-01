using System.Security.Claims;
using Fga.Example.AspNetCore;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
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
builder.Services.AddOpenFga(clientConfig =>
{
    clientConfig.WithAuth0FgaDefaults(builder.Configuration["Auth0Fga:ClientId"], builder.Configuration["Auth0Fga:ClientSecret"]);
    clientConfig.StoreId = builder.Configuration["Auth0Fga:StoreId"];
  
}, middlewareConfig =>
{
    middlewareConfig.UserIdentityResolver = principal => principal.Identity!.Name!;
});


// OpenFGA
/*builder.Services.AddOpenFga(x =>
{
    x.ApiScheme = builder.Configuration["Fga:ApiScheme"];
    x.ApiHost = builder.Configuration["Fga:ApiHost"];
    x.StoreId = builder.Configuration["Fga:StoreId"];
});*/

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
    .WithMetadata(new FgaRouteObjectAttribute("viewer", "doc", "sdfsdfsdf"));

app.Run();
