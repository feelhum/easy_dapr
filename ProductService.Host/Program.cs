using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

// ע�� IFreeSql
builder.Services.AddSingleton<IFreeSql>(provider =>
    new FreeSqlBuilder()
        .UseConnectionString(DataType.MySql, "server=192.168.4.73;port=3306;database=test_dapr;uid=root;pwd=Ks.@123456;SslMode=None;Pooling=True;Max Pool Size=5000;Min Pool Size=3;Connection Lifetime=30;") // �滻Ϊ������ݿ����ͺ������ַ���
        .UseAutoSyncStructure(true)

        .Build());

// ���� ApiBehaviorOptions��ģ�� [ApiController] �Ĺ���
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = false; // �Զ�����ģ����֤����
//});
// ��� MVC ����ע���Զ�����������ֹ���
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new AppServiceControllerModelConvention());
    options.Conventions.Add(new CustomControllerConvention());
    options.Conventions.Add(new CustomRouteConvention());
    options.Filters.Add<UnifiedResponseFilter>(); // ע��ȫ�ֹ�����
});

// ע�� DaprClient ������ע������
builder.Services.AddDaprClient();

// ���� ApiBehaviorOptions
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false; // �Զ�����ģ����֤����
});

var app = builder.Build();
// ע��ȫ���쳣�����м��
app.UseMiddleware<GlobalExceptionMiddleware>();
// ��ӡ����ע���·����Ϣ
app.Use(async (context, next) =>
{
    var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
    foreach (var endpoint in endpointDataSource.Endpoints)
    {
        Console.WriteLine($"Registered Endpoint: {endpoint.DisplayName}");
    }
    await next();
});
app.UseRouting(); // ������ UseRouting()��������ƥ��·��
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    Console.WriteLine($"Endpoint: {endpoint?.DisplayName}");
    await next();
});
// ʹ�ÿ�����ӳ��·��
app.MapControllers();

app.Run();