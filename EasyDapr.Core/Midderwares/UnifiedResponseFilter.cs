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
                var daprIdentity = context.HttpContext.Request.Headers["traceparent"].FirstOrDefault();

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

            // 判断是否是来自 Dapr 的内部调用
            var isInternalDaprCall = context.HttpContext.Request.Headers["traceparent"].FirstOrDefault() != null;

            if (isInternalDaprCall)
            {
                // 如果是内部调用，不进行包装，直接返回原始结果
                return;
            }

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
                        errorMessage = "An unexpected error occurred. Please try again later （1）.",
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

    }
}
