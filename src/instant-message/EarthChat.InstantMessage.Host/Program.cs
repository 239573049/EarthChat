using EarthChat.Gateway.Sdk.Extensions;
using EarthChat.Scalar.Extensions;
using EarthChat.Serilog.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseGatewayNode();

builder.Services.WithScalar();
builder.AddServiceDefaults();

builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseScalar("EarthChat Instant Message Service");
}

app.MapDefaultEndpoints();

app.Run();