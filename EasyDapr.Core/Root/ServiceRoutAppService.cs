using EasyDapr.Core.Midderwares;
using EasyDapr.Core.Root.Models;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Root
{
    public class ServiceRoutAppService : EasyDaprService, IServiceRoutAppService
    {
        private readonly EndpointDataSource _endpointDataSource;

        public ServiceRoutAppService(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }
        public ICollection<RouteInfo> GetRoutes()
        {
            var routes = _endpointDataSource.Endpoints
           .OfType<RouteEndpoint>();

            var result = new List<RouteInfo>();

            foreach (var route in routes)
            {
                if ((route.RoutePattern.RawText ?? "").ToLower().Contains("ServiceRout/getroutes".ToLower()))
                    continue;

                // 获取控制器和方法信息
                var actionDescriptor = route.Metadata
                    .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
                    .FirstOrDefault();

                if (actionDescriptor == null) continue;

                var methodInfo = actionDescriptor.MethodInfo;

                // 获取返回类型
                var outType = GetFriendlyTypeName(methodInfo.ReturnType);

                // 获取参数类型
                var inTypes = methodInfo.GetParameters()
                    .Select(p => new ParameterInfo
                    {
                        Name = p.Name ?? "",
                        Type = GetFriendlyTypeName(p.ParameterType)
                    }).ToList();
                // 获取类名和方法名
                var className = methodInfo.DeclaringType?.Name;
                var methodName = methodInfo.Name;
                var x = route.Metadata.OfType<HttpMethodMetadata>();
                result.Add(new RouteInfo()
                {
                    HttpMethods = route.Metadata.OfType<HttpMethodMetadata>()?.FirstOrDefault()?.HttpMethods.ToList() ?? new List<string>(),
                    Route = route.RoutePattern.RawText ?? "",
                    ReturnType = outType,
                    Parameters = inTypes,
                    ClassName = className ?? "",
                    MethodName = methodName

                });
            }

            return result;


        }


        // 获取友好的类型名称（包括泛型类型）
        private string GetFriendlyTypeName(Type type)
        {
            // 定义常见类型的映射字典
            var typeMappings = new Dictionary<string, string>
    {
        { "String", "string" },
        { "Boolean", "bool" },
        { "Int32", "int" },
        { "Int64", "long" },
        { "Double", "double" },
        { "Decimal", "decimal" },
        { "Char", "char" },
        { "Byte", "byte" },
        { "Void", "void" },
        { "Object", "object" }
    };

            // 如果是泛型 Task<>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // 返回 Task 的泛型参数类型
                return GetFriendlyTypeName(type.GetGenericArguments()[0]);
            }

            // 如果是非泛型 Task
            if (type == typeof(Task))
            {
                return "void";
            }

            // 如果是可空类型 Nullable<>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // 返回可空类型的基础类型
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}?";
            }

            // 如果是数组类型
            if (type.IsArray)
            {
                // 获取数组元素类型并加上 "[]"
                return $"{GetFriendlyTypeName(type.GetElementType())}[]";
            }

            // 如果是普通泛型类型
            if (type.IsGenericType)
            {
                var genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
                return $"{type.Name.Substring(0, type.Name.IndexOf('`'))}<{genericArguments}>";
            }

            // 普通类型直接返回类型名称，映射为常见类型
            if (typeMappings.TryGetValue(type.Name, out var friendlyName))
            {
                return friendlyName;
            }

            // 如果没有匹配到映射，则返回原始类型名称
            return type.Name;
        }
    }
}
