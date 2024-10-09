using System.Collections.ObjectModel;
using EarthChat.Infrastructure.Gateway;
using EarthChat.Infrastructure.Gateway.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
builder.Services.AddSwaggerGen();
builder.Services.AddNodeService();
builder.Services.AddGateway(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGatewayMiddleware();
app.MapNodeService();

app.Run();