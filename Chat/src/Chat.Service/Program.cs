using Chat.Service.Infrastructure.Expressions;
using Chat.Service.Options;
using FreeRedis;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddDomainEventBus(options =>
{
    options.UseRepository<ChatDbContext>();
}).AddAutoMapper(typeof(Program));

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);

var app = builder.Services
    .AddEndpointsApiExplorer()
    .AddJwtBearerAuthentication(jwtSection.Get<JwtOptions>())
    .AddSingleton(_ => new RedisClient(builder.Configuration["Redis"]))
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "ChatApp", Version = "v1",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "ChatApp", }
            });
        foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml"))
            options.IncludeXmlComments(item, true);
        options.DocInclusionPredicate((docName, action) => true);
    })
    .AddEventBus()
    .AddMasaDbContext<ChatDbContext>(opt => { opt.UseNpgsql(); })
    .AddAutoInject()
    .AddServices(builder, option => option.MapHttpMethodsForUnmatched = new[] { "Post" });

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
app.UseAuthorization();

// 解决pgsql的时间戳问题
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await app.RunAsync();