using Dapr.Client;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.ResponseResult;
using Newtonsoft.Json;
namespace EasyDapr.Core.Extensions
{
    public static class DaprClientExtension
    {
        public static async Task<TResponse> EasyInvokeMethodAsync<TRequest, TResponse>(this DaprClient daprClient, HttpMethod httpMethod, string appId, string methodName, TRequest data, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // 调用目标服务的 gRPC 方法
                var result = await daprClient.InvokeMethodAsync<TRequest, TResponse>(
                    httpMethod: httpMethod,  // HTTP 方法
                    appId,          // 目标服务的 AppId
                    methodName,           // 目标服务的方法名
                    data           // 请求数据
                );
                return result;
            }
            catch (Dapr.Client.InvocationException daprEx)
            {
                // 解析目标服务返回的错误内容
                var errorResponse = await daprEx.Response.Content.ReadAsStringAsync();


                var dto = Newtonsoft.Json.JsonConvert.DeserializeObject<StandardApiResponse<string>>(errorResponse);
                // 打印错误信息
                Console.WriteLine($"Error from target service: {errorResponse}");

                // 抛出自定义异常
                throw new UserFriendlyException($"当前错误:{daprEx.Message} ; 内部错误:{dto?.ErrorMessage}");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex!.Message+ex.InnerException?.Message??"");
            }

        }
    }
}
