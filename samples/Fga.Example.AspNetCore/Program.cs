using System.Security.Claims;
using Fga.Example.AspNetCore;
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
builder.Services.AddOpenFga(x =>
{
    x.WithAuth0FgaDefaults(builder.Configuration["Auth0Fga:ClientId"], builder.Configuration["Auth0Fga:ClientSecret"]);

    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
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

app.MapGet("/viewminimal/{documentId}",
        [EntityAuthorization("doc", "documentId")]
    (documentId) => Task.FromResult(documentId))
    .RequireAuthorization(FgaAuthorizationDefaults.PolicyKey);

app.Run();
