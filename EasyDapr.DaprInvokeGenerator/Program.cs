using EasyDapr.Core.ResponseResult;
using System;
using System.CommandLine;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DaprInvokeGenerator
{
    /// <summary>
    /// 全局命令安装：dotnet tool install -g EasyDapr.DaprInvokeGenerator --add-source D:\mydapr\EasyDapr\EasyDapr.DaprInvokeGenerator\bin\Debug
    /// 生成工具命令：easydapr --appid my-service --getroutes-url http://localhost:4000/api/ServiceRout/getroutes
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // 提示用户输入 Dapr 服务的 AppId
            Console.WriteLine("Please enter the Dapr service AppId:");
            string? appId = Console.ReadLine();
            if (string.IsNullOrEmpty(appId))
            {
                Console.WriteLine("Error: AppId is required.");
                return;
            }

            // 提示用户输入 GetRoutes API 的 URL
            Console.WriteLine("Please enter the URL of the GetRoutes API:");
            string? getRoutesUrl = Console.ReadLine();
            if (string.IsNullOrEmpty(getRoutesUrl))
            {
                Console.WriteLine("Error: GetRoutes URL is required.");
                return;
            }

            // 提示用户输入输出目录（可选，默认当前目录的 Invokes 文件夹）
            Console.WriteLine("Please enter the output directory (leave empty for default 'Invokes' folder in the current directory):");
            string? outputDir = Console.ReadLine();
            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Invokes");
            }
            else
            {
                outputDir = Path.Combine(outputDir, "Invokes"); 
            }

            Directory.CreateDirectory(outputDir);
            Console.WriteLine($"Files will be generated in: {outputDir}");

            try
            {
                // 从指定的 GetRoutes API 获取路由信息
                Console.WriteLine($"Fetching routes from: {getRoutesUrl}");
                using var httpClient = new HttpClient();
                var routes = await httpClient.GetFromJsonAsync<StandardApiResponse<List<RouteInfo>>>(getRoutesUrl);

                if (routes == null)
                {
                    Console.WriteLine("No routes found.");
                    return;
                }

                if (routes.Data == null)
                {
                    Console.WriteLine("No routes found.");
                    return;
                }

                if (routes.Data.Count==0)
                {
                    Console.WriteLine("No routes found.");
                    return;
                }

                // 为每个类生成对应的文件
                foreach (var group in routes.Data.GroupBy(r => r.ClassName))
                {
                    string className = group.Key;
                    string filePath = Path.Combine(outputDir, $"{className}.cs");

                    var fileContent = GenerateClassFile(appId, className, group.ToList());
                    await File.WriteAllTextAsync(filePath, fileContent);

                    Console.WriteLine($"Generated: {filePath}");
                }

                Console.WriteLine("Code generation completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private static string GetParameters(List<ParameterInfo> parameters)
        {
            var parametersStr = string.Join(", ", parameters.Select(p => $"{p.Name}"));
            if (parameters.Count > 1)
                parametersStr = $"new {{ {parametersStr} }}";
            return parametersStr;
        }

        private static string GetReturns(List<ParameterInfo> parameters)
        {
            var parametersStr = string.Join(", ", parameters.Select(p => $"{p.Type}"));
            if (parameters.Count > 1)
                parametersStr = "object";
            return parametersStr;
        }
        static  string GenerateClassFile(string appId, string className, List<RouteInfo> routes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using Dapr.Client;");
            sb.AppendLine($"using {appId}Service.Dtos;");
            sb.AppendLine("using EasyDapr.Core.Dtos;");
            sb.AppendLine("using EasyDapr.Core.Extensions;");
            sb.AppendLine();
            sb.AppendLine($"namespace Invokes");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {className}");
            sb.AppendLine("    {");

            foreach (var route in routes)
            {
                sb.AppendLine($"        public static async Task<{route.ReturnType}> {route.MethodName}Async(this DaprClient daprClient, {string.Join(", ", route.Parameters.Select(p => $"{p.Type} {p.Name}"))})");
                sb.AppendLine("        {");
                sb.AppendLine($"            string targetAppId = \"{appId}\";");
                sb.AppendLine($"            string methodName = \"{route.Route}\";");
                sb.AppendLine();
                sb.AppendLine($"            return await daprClient.EasyInvokeMethodAsync<{GetReturns(route.Parameters)}, {route.ReturnType}>(HttpMethod.{ToPascalCase(route.HttpMethods.First())}, targetAppId, methodName,  {GetParameters(route.Parameters)});");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // 将字符串转换为 PascalCase（首字母大写）
        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }

    public class RouteInfo
    {
        public string Route { get; set; }
        public List<string> HttpMethods { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}