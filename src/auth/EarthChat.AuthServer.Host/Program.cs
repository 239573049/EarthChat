using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.Gateway.Sdk.Extensions;
using EarthChat.Jwt.Extensions;
using EarthChat.Scalar.Extensions;
using EarthChat.Serilog.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.WithScalar();

builder.WebHost.UseGatewayNode();

builder.AddServiceDefaults();

builder.Services.AddJwt(builder.Configuration);

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

builder.Services.AddAutoGnarly();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
	app.UseScalar("EarthChat Auth Server");
}
await app.RunAsync();