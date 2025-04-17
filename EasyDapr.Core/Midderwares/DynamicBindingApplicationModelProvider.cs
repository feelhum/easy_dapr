using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Midderwares
{
    public class DynamicBindingApplicationModelProvider : IApplicationModelProvider
    {
        public int Order => -1000; // 确保在默认逻辑之前执行

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            Console.WriteLine($"注入 OnProvidersExecuting:{context.Result.Controllers.Count}");
            foreach (var controller in context.Result.Controllers)
            {
                Console.WriteLine($"Processing controller: {controller.ControllerName}");
                foreach (var action in controller.Actions)
                {
                    Console.WriteLine($"  Processing action: {action.ActionName}");
                    foreach (var parameter in action.Parameters)
                    {
                        Console.WriteLine($"    Processing parameter: {parameter.ParameterName} of type {parameter.ParameterType}");
                        // 如果参数已经有绑定信息，跳过
                        if (parameter.BindingInfo != null)
                            continue;

                        // 根据类型动态设置绑定方式
                        if (IsComplexType(parameter.ParameterType))
                        {
                            parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { BindingSource.Body });
                            Console.WriteLine($"      Binding {parameter.ParameterName} to Body");
                        }
                        else if (IsSimpleType(parameter.ParameterType))
                        {
                            parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { BindingSource.Query });
                            Console.WriteLine($"      Binding {parameter.ParameterName} to Query");
                        }
                    }
                }
            }
        }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            // 不需要实现
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
                   type == typeof(DateTime) || Nullable.GetUnderlyingType(type) != null;
        }

        private bool IsComplexType(Type type)
        {
            return type.IsClass && type != typeof(string);
        }
    }
}
