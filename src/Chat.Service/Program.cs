using Microsoft.Extensions.DependencyInjection;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 解决pgsql的时间戳问题
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // 从配置文件中读取Serilog配置
    .CreateLogger();

builder.Host.UseSerilog(); // 将Serilog配置到Host中

builder.Services.AddSignalR()
    .AddMessagePackProtocol()
    .AddStackExchangeRedis(builder.Configuration["ConnectionStrings:Redis"],
        options => { options.Configuration.ChannelPrefix = "Chat:"; });

#region Options

var jwtSection = builder.Configuration.GetSection("Jwt");
var chatGpt = builder.Configuration.GetSection(Constant.ChatGPT);
var github = builder.Configuration.GetSection("GitHub");
var gitee = builder.Configuration.GetSection("Gitee");

builder.Services.Configure<ChatGptOptions>(chatGpt);
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<GithubOptions>(github);
builder.Services.Configure<GiteeOptions>(gitee);

#endregion

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>
    (builder.Configuration.GetSection("IpRateLimit"));

builder.Services.AddSingleton<IRateLimitConfiguration,
    RateLimitConfiguration>();

builder.Services.AddInMemoryRateLimiting();

builder.Services.AddHttpClient(Constant.ChatGPT, (services,c) =>
{
    var options = services.GetRequiredService<IOptions<ChatGptOptions>>().Value;
    c.BaseAddress =new Uri(options.Url);
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "Chat");
    c.DefaultRequestHeaders.Add("Authorization","Bearer "+ options.Token);
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
                Title = "ChatApp",
                Version = "v1",
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
}

#region MigrationDb

await using var context = app.Services.CreateScope().ServiceProvider.GetService<ChatDbContext>();
{
    try
    {
        // 执行sql
        await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION hstore;");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    // 判断是否需要创建数据库
    context!.Database.EnsureCreated();
}

#endregion

app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization().UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chatHub");

await app.RunAsync();
Log.CloseAndFlush();