using Fga.Net.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuth0Fga(x =>
{
    x.ClientId = builder.Configuration["Auth0Fga:ClientId"];
    x.ClientSecret = builder.Configuration["Auth0Fga:ClientSecret"];
    x.StoreId = builder.Configuration["Auth0Fga:StoreId"];
});


builder.Services.AddAuthorization(options =>
{
    options.AddFgaPolicy();
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


//app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
