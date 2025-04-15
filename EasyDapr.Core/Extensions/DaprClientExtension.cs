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

                var msgSplied = dto?.ErrorMessage.GetMessageSplied();
                // 抛出自定义异常 【{msgSplied?.MessageDetail}=>{daprEx.Message}】
                throw new UserFriendlyException($"{msgSplied?.Message}");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex!.Message+ex.InnerException?.Message??"");
            }

        }
        
        public static MessageSplied GetMessageSplied(this string message)
        {
            var msgSplit = message.Split("【");
            return new MessageSplied() { Message = msgSplit[0], MessageDetail = (msgSplit.Length > 1 ? msgSplit[1] : "").Replace("】", "").Replace("=>", "") };
        }

    }

    public class MessageSplied
    {
        public string Message { get; set; }
        public string  MessageDetail { get; set; }
    }
}
