using Dapr.Client;
using EasyDapr.Core.Grpc;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text.Json;

namespace EasyDapr.Core.Exceptions
{
    public static class DaprClientExtensions
    {
        public static async Task<TResponse> InvokeGrpcWithAutoSerializationAsync<TRequest, TResponse>(
            this DaprClient client,
            string appId,
            string methodName,
            TRequest request)
        {
            // 包装输入参数
            IMessage wrappedRequest = WrapRequest(request);

            // 调用 gRPC 方法
            var wrappedResponse = await client.InvokeMethodGrpcAsync<IMessage, Any>(
                appId,
                methodName,
                wrappedRequest
            );

            // 解包返回值
            return UnwrapResponse<TResponse>(wrappedResponse);
        }

        private static IMessage WrapRequest<TRequest>(TRequest request)
        {
            if (request is IMessage message)
            {
                // 如果输入已经是 IMessage，则直接返回
                return message;
            }

            return new DynamicMessageWrapper(request);
        }

        private static TResponse UnwrapResponse<TResponse>(Any response)
        {
            // 将 Any 类型转换为目标类型
            var json = response.Value.ToStringUtf8();
            return JsonSerializer.Deserialize<TResponse>(json);
        }
    }
}
