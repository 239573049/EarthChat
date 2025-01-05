using EarthChat.Gateway.Sdk;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGatewayService(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
