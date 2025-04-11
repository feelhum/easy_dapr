using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

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

// 注册 DaprClient 到依赖注入容器
builder.Services.AddDaprClient();

// 启用 ApiBehaviorOptions
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false; // 自动返回模型验证错误
});

var app = builder.Build();
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