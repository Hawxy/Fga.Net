using System.Security.Claims;
using Fga.Example.AspNetCore;
using Fga.Net.AspNetCore;
using Fga.Net.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

builder.Services.AddAuth0Fga(x =>
{
    x.ClientId = builder.Configuration["Auth0Fga:ClientId"];
    x.ClientSecret = builder.Configuration["Auth0Fga:ClientSecret"];
    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
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

app.MapGet("/viewminimal/{documentId}",
    [Authorize(FgaAuthorizationDefaults.PolicyKey)] 
    [EntityAuthorization("doc", "documentId")]
    (documentId) => Task.FromResult(documentId));

app.Run();
