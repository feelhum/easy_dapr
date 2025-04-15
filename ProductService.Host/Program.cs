using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.AppService;
var builder = WebApplication.CreateBuilder(args);

// 添加 Swagger 服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "An example API for Dapr integration with FreeSql and custom conventions"
    });

    // 添加对使用 `Bearer` Token 的支持（如果需要授权，可以添加如下配置）
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 注册 IFreeSql
builder.Services.AddSingleton<IFreeSql>(provider =>
    new FreeSqlBuilder()
        .UseConnectionString(DataType.MySql, "server=192.168.4.73;port=3306;database=test_dapr;uid=root;pwd=Ks.@123456;SslMode=None;Pooling=True;Max Pool Size=5000;Min Pool Size=3;Connection Lifetime=30;") // 替换为你的数据库类型和连接字符串
        .UseAutoSyncStructure(true)

        .Build());

// 启用 ApiBehaviorOptions，模拟 [ApiController] 的功能
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = false; // 自动返回模型验证错误
//});
// 添加 MVC 服务并注册自定义控制器发现规则
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new AppServiceControllerModelConvention());
    options.Conventions.Add(new CustomControllerConvention());
    options.Conventions.Add(new CustomRouteConvention());
    options.Filters.Add<UnifiedResponseFilter>(); // 注册全局过滤器
});



// 启用 ApiBehaviorOptions
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false; // 自动返回模型验证错误
});
// 注册 DaprClient 到依赖注入容器
builder.Services.AddDaprClient();
var app = builder.Build();

// 启用 Swagger 中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // 设置 Swagger UI 的根路径（默认是 `/`）
    });
}

// 注册全局异常处理中间件
app.UseMiddleware<GlobalExceptionMiddleware>();
// 打印所有注册的路由信息
app.Use(async (context, next) =>
{
    var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
    foreach (var endpoint in endpointDataSource.Endpoints)
    {
        Console.WriteLine($"Registered Endpoint: {endpoint.DisplayName}");
    }
    await next();
});
app.UseRouting(); // 必须有 UseRouting()，它负责匹配路由
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    Console.WriteLine($"Endpoint: {endpoint?.DisplayName}");
    await next();
});
// 使用控制器映射路由
app.MapControllers();
app.Run();