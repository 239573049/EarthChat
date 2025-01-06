using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.Gateway.Sdk.Extensions;
using EarthChat.Serilog.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseGatewayNode();

builder.AddServiceDefaults();

builder.Services.WithAuthDbAccess((optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"));

#if DEBUG
    optionsBuilder.EnableSensitiveDataLogging();
    optionsBuilder.EnableDetailedErrors();
#endif
}));

builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddAutoGnarly();

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