using System.Collections.ObjectModel;
using EarthChat.Infrastructure.Gateway;
using EarthChat.Infrastructure.Gateway.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromMemory(new Collection<RouteConfig>(), new Collection<ClusterConfig>())
    .ConfigureHttpClient(((context, handler) =>
    {
        handler.SslOptions.RemoteCertificateValidationCallback =
            (sender, certificate, chain, errors) => true;
    }))
    .AddServiceDiscoveryDestinationResolver();

builder.WebHost.UseKestrel(options =>
{
    // 使用http2
    options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddNodeService();
builder.Services.AddGateway(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference((options =>
    {
        options.Title = "EarthChat Gateway";
        options.Authentication = new ScalarAuthenticationOptions()
        {
            PreferredSecurityScheme = "Bearer",
        };
    }));
}


app.UseGatewayMiddleware();
app.MapNodeService();

app.Run();