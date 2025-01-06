using EarthChat.Gateway.Sdk;
using EarthChat.Gateway.Sdk.Extensions;
using EarthChat.Serilog.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseGatewayNode();

builder.AddServiceDefaults();

builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference((options =>
    {
        options.Title = "EarthChat Instant Message Service";
        options.Authentication = new ScalarAuthenticationOptions()
        {
            PreferredSecurityScheme = "Bearer",
        };
    }));
}

app.MapDefaultEndpoints();

app.Run();