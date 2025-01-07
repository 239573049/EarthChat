using EarthChat.AuthServer.Application.Extensions;
using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.AuthServer.Host.Apis;
using EarthChat.Gateway.Sdk.Extensions;
using EarthChat.Jwt.Extensions;
using EarthChat.Scalar.Extensions;
using EarthChat.Serilog.Extensions;
using Token.RabbitMQEvent;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.WithGatewayNode();

builder.AddServiceDefaults();

builder.Services
    .WithScalar()
    .WithAuthApplicationServices(builder.Configuration)
    .WithRabbitMqEventBus(builder.Configuration)
    .WithRabbitMqJsonSerializer()
    .WithJwt(builder.Configuration)
    .WithAuthDbAccess(builder.Configuration)
    .WithSerilog(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoGnarly();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapVerificationApis();

if (app.Environment.IsDevelopment())
{
    app.UseScalar("EarthChat Auth Server");
}

await app.RunAsync();