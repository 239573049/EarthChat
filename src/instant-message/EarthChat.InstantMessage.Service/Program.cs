using EarthChat.Gateway.Sdk;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddGatewayService(builder.Configuration);

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