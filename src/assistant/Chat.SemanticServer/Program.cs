using System.Reflection;
using System.Text.Json.Serialization;
using Chat.SemanticServer.Options;
using FluentValidation.AspNetCore;
using Infrastructure.JsonConverters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Swashbuckle.AspNetCore.SwaggerGen;

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SK API", Version = "v1" });
    //添加Api层注释（true表示显示控制器注释）
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    //添加Domain层注释（true表示显示控制器注释）
    var xmlFile1 = $"{Assembly.GetExecutingAssembly().GetName().Name.Replace("Api", "Domain")}.xml";
    var xmlPath1 = Path.Combine(AppContext.BaseDirectory, xmlFile1);
    c.IncludeXmlComments(xmlPath1, true);
    c.DocInclusionPredicate((docName, apiDes) =>
    {
        if (!apiDes.TryGetMethodInfo(out MethodInfo method))
            return false;
        var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>()
            .Select(m => m.GroupName);
        if (docName == "v1" && !version.Any())
            return true;
        var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>()
            .Select(m => m.GroupName);
        if (actionVersion.Any())
            return actionVersion.Any(v => v == docName);
        return version.Any(v => v == docName);
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description =
            "Directly enter bearer {token} in the box below (note that there is a space between bearer and token)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference()
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
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

builder.Services.AddTransient<IKernel>((_) =>
    {
        return Kernel.Builder
            .WithAzureChatCompletionService(
                OpenAIOptions.Model,
                OpenAIOptions.Endpoint,
                OpenAIOptions.Key)
            .Build();
    });

var app = builder.Build();

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