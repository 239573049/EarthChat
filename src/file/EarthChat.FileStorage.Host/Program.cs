using EarthChat.Scalar.Extensions;
using EarthChat.Serilog.Extensions;
using EarthChat.Gateway.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.WithGatewayNode();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .WithScalar()
    .WithSerilog(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoGnarly();

builder.Services.AddGatewayNode(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseScalar("EarthChat File Storage");
}

app.Run();