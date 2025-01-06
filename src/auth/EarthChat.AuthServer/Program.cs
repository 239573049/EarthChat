using EarthChat.Gateway.Sdk.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseGatewayNode();

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference((options =>
    {
        options.Title = "EarthChat Auth Server";
        options.Authentication = new ScalarAuthenticationOptions()
        {
            PreferredSecurityScheme = "Bearer",
        };
    }));
}


await app.RunAsync();