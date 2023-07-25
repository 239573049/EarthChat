using System.Text.Json;
using AspNetCoreRateLimit;
using Chat.Service.Hubs;
using Chat.Service.Infrastructure.Extensions;
using Chat.Service.Options;
using FreeRedis;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
    .AddMessagePackProtocol()
    .AddStackExchangeRedis(builder.Configuration["ConnectionStrings:Redis"],
        options => { options.Configuration.ChannelPrefix = "Chat:"; });

#region Options

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);

var github = builder.Configuration.GetSection("GitHub");
var gitee = builder.Configuration.GetSection("Gitee");

builder.Services.Configure<GithubOptions>(github);
builder.Services.Configure<GiteeOptions>(gitee);

#endregion

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>
    (builder.Configuration.GetSection("IpRateLimit"));

builder.Services.AddSingleton<IRateLimitConfiguration,
    RateLimitConfiguration>();

builder.Services.AddInMemoryRateLimiting();

builder.Services
    .AddHttpClient("Github", c =>
    {
        c.DefaultRequestHeaders.Add("Accept", "application/json");
        c.DefaultRequestHeaders.Add("User-Agent", "Chat");
    });

var app = builder.Services
    .AddEndpointsApiExplorer()
    .AddAutoMapper(typeof(Program))
    .AddJwtBearerAuthentication(jwtSection.Get<JwtOptions>())
    .AddSingleton(_ =>
    {
        var client = new RedisClient(builder.Configuration["ConnectionStrings:Redis"]);
        client.Serialize = o => JsonSerializer.Serialize(o);
        client.Deserialize = (s, t) => JsonSerializer.Deserialize(s, t);
        return client;
    })
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "ChatApp", Version = "v1",
                Contact = new OpenApiContact { Name = "ChatApp" }
            });
        foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml"))
            options.IncludeXmlComments(item, true);
        options.DocInclusionPredicate((docName, action) => true);
    })
    .AddEventBus()
    .AddMasaDbContext<ChatDbContext>(opt =>
    {
        opt.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
    })
    .AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", corsBuilder =>
        {
            corsBuilder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader()
                .AllowCredentials();
        });
    })
    .AddDomainEventBus(options =>
    {
        options.UseUoW<ChatDbContext>()
            .UseRepository<ChatDbContext>();
    })
    .AddAutoInject()
    .AddServices(builder, option => option.MapHttpMethodsForUnmatched = new[] { "Post" });


app.UseIpRateLimiting();

app.UseMasaExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatApp"));

    #region MigrationDb

    await using var context = app.Services.CreateScope().ServiceProvider.GetService<ChatDbContext>();
    {
        context!.Database.EnsureCreated();
    }

    #endregion
}

app.UseAuthentication();
app.UseAuthorization().UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chatHub");

// 解决pgsql的时间戳问题
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await app.RunAsync();