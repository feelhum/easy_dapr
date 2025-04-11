using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Attributes;
using System.Net;

namespace EasyDapr.Core.Midderwares
{
    public class UnifiedResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasAttribute = context.ActionDescriptor.EndpointMetadata
                .OfType<InternalAccessOnlyAttribute>()
                .Any();

            if (hasAttribute)
            {
                // 验证请求头中的 X-Dapr-Identity
                var daprIdentity = context.HttpContext.Request.Headers["X-Dapr-Identity"].FirstOrDefault();

                if (string.IsNullOrEmpty(daprIdentity))
                {
                    context.Result = new ObjectResult(new
                    {
                        data = null as object,
                        status = 403,
                        code = "Failure",
                        errorMessage = "This endpoint is restricted to internal services only；此接口只允许服务间内部访问。",
                        validErrors = null as object
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                if (context.Exception is UserFriendlyException userFriendlyException)
                {
                    // 如果是 UserFriendlyException，按其状态码包装返回值
                    context.Result = new ObjectResult(new
                    {
                        data = null as object,
                        status = userFriendlyException.StatusCode,
                        code = "Failure",
                        errorMessage = userFriendlyException.Message,
                        validErrors = null as object
                    })
                    {
                        StatusCode = userFriendlyException.StatusCode
                    };
                }
                else
                {
                    // 其他异常，按默认的 500 状态码处理
                    context.Result = new ObjectResult(new
                    {
                        data = null as object,
                        status = 500,
                        code = "Failure",
                        errorMessage = "An unexpected error occurred. Please try again later.",
                        validErrors = null as object
                    })
                    {
                        StatusCode = 500
                    };
                }

                // 标记异常已处理
                context.ExceptionHandled = true;
                return;
            }

            // 如果操作结果是 ObjectResult，则进行统一包装
            if (context.Result is ObjectResult objectResult)
            {
                context.Result = new ObjectResult(new
                {
                    data = objectResult.Value,
                    status = 200,
                    code = "Success",
                    errorMessage = null as string,
                    validErrors = null as object
                })
                {
                    StatusCode = objectResult.StatusCode ?? 200
                };
            }
            // 如果操作结果是 null，则返回默认响应
            else if (context.Result == null)
            {
                context.Result = new ObjectResult(new
                {
                    data = null as object,
                    status = 200,
                    code = "Success",
                    errorMessage = null as string,
                    validErrors = null as object
                })
                {
                    StatusCode = 200
                };
            }
        }

        private bool IsRequestFromDaprSidecar(IPAddress remoteIpAddress)
        {
            // 检查请求是否来自本地主机
            return remoteIpAddress != null && (IPAddress.IsLoopback(remoteIpAddress) || remoteIpAddress.Equals(IPAddress.IPv6Loopback));
        }

        private bool IsInternalRequest(ActionExecutingContext context)
        {
            // 检查 Dapr-App-Id 是否存在（用于识别服务间调用）
            var daprAppId = context.HttpContext.Request.Headers["Dapr-App-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(daprAppId))
            {
                return true; // 是服务间调用
            }

            // 检查是否是 RPC 请求（通过 gRPC 标识）
            var isGrpcRequest = context.HttpContext.Request.Headers.ContainsKey("grpc-trace-bin");
            if (isGrpcRequest)
            {
                return true; // 是 RPC 服务间调用
            }

            // 如果没有以上标识，则认为是外部客户端调用
            return false;
        }
    }
}
