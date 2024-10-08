using EarthChat.Infrastructure.Gateway.Sdk;
using EarthChat.Infrastructure.Gateway.Sdk.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var gatewayServiceOptions = builder.Configuration.GetSection("Gateway").Get<GatewayServiceOptions>();

builder.Services.AddGatewayService(options =>
{
    options.Service = gatewayServiceOptions.Service;
    options.Address = gatewayServiceOptions.Address;
    options.Port = gatewayServiceOptions.Port;
    options.Ip = gatewayServiceOptions.Ip;
    options.Token = gatewayServiceOptions.Token;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}