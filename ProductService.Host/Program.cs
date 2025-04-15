using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ProductService.AppService;
var builder = WebApplication.CreateBuilder(args);

// ��� Swagger ����
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "An example API for Dapr integration with FreeSql and custom conventions"
    });

    // ��Ӷ�ʹ�� `Bearer` Token ��֧�֣������Ҫ��Ȩ����������������ã�
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



// ���� ApiBehaviorOptions
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false; // �Զ�����ģ����֤����
});
// ע�� DaprClient ������ע������
builder.Services.AddDaprClient();
var app = builder.Build();

// ���� Swagger �м��
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // ���� Swagger UI �ĸ�·����Ĭ���� `/`��
    });
}

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