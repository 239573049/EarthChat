using System.Reflection;
using System.Text.Json.Serialization;
using Chat.SemanticServer.Options;
using FluentValidation.AspNetCore;
using Infrastructure.JsonConverters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
    options.SerializerOptions.Converters.Add(new DateTimeConverter());
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new DateTimeNullableConvert());
});

builder.Configuration.GetSection("OpenAI").Get<OpenAIOptions>();

builder.Services
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
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader()
            .AllowCredentials();
    });
});

//模型验证
builder.Services.AddControllersWithViews()
    .AddFluentValidation(config => //添加FluentValidation验证
    {
        //程序集方式添加验证
        // config.RegisterValidatorsFromAssemblyContaining(typeof(TMTotalCostGenerateDetailValidation));
        //注入程序集
        config.RegisterValidatorsFromAssembly(Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name));
        config.RegisterValidatorsFromAssembly(Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name
            .Replace("Api", "Domain")));
        //是否与MvcValidation共存，设置为false后将不再执行特性方式的验证
        //config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
    }).Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = (context) =>
        {
            var errors = context.ModelState
                .Values
                .SelectMany(x => x.Errors
                    .Select(p => p.ErrorMessage))
                .ToList();

            var result = new
            {
                code = "400",
                message = "Validation errors",
                data = errors
            };

            return new BadRequestObjectResult(result);
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("ChatGPT", (services, c) =>
{
    c.DefaultRequestHeaders.Add("X-Token", "token");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "Chat");
});

builder.Services.AddTransient<IKernel>((services) =>
{
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    return Kernel.Builder
        .WithOpenAIChatCompletionService(
            OpenAIOptions.Model,
            OpenAIOptions.Key,
            httpClient: httpClientFactory.CreateClient("ChatGPT"))
        .Build();
});


var app = builder.Services.AddServices(builder, options =>
{
    options.MapHttpMethodsForUnmatched = new[] { "Post" }; //当请求类型匹配失败后，默认映射为Post请求 (当前项目范围内，除非范围配置单独指定)
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization()
    .UseCors("CorsPolicy");

app.MapControllers();

app.Run();