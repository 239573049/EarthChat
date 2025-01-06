using System.Collections.ObjectModel;
using EarthChat.Infrastructure.Gateway;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromMemory(new Collection<RouteConfig>(), new Collection<ClusterConfig>())
    .AddTransforms((context =>
    {
        var prefix = context.Route.Match.Path?.Replace("/{**catch-all}", "");
        if (!string.IsNullOrEmpty(prefix))
        {
            if (prefix.StartsWith('/'))
                context.AddPathRemovePrefix(prefix);
        }
        
        // 添加原始主机
        context.AddOriginalHost(true);
        
    }))
    .ConfigureHttpClient(((context, handler) =>
    {
        handler.SslOptions.RemoteCertificateValidationCallback =
            (sender, certificate, chain, errors) => true;
        // 尽可能复用
        handler.EnableMultipleHttp2Connections = true;
        handler.MaxConnectionsPerServer = 10;
        handler.InitialHttp2StreamWindowSize = 5;
    }));

builder.WebHost.UseKestrel(options =>
{
    // 使用http2
    options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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

app.MapReverseProxy();

app.UseGatewayMiddleware();


app.Run();