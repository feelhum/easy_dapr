using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Midderwares
{
    public class CustomRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers.Where(t => t.ControllerType.Name.EndsWith("AppService")))
            {
                // 去掉 "AppService" 后缀并将控制器名称转换为小写
                // var controllerName = controller.ControllerType.Name.ToLower().Substring(0, controller.ControllerType.Name.Length - "AppService".Length);

                foreach (var action in controller.Actions)
                {
                    
                    var selector = action.Selectors.FirstOrDefault()?? new SelectorModel();

                    var attributeRouteModel = selector?.AttributeRouteModel;
                    var actionConstraints = selector?.ActionConstraints;

                    if(attributeRouteModel!=null && actionConstraints?.Count>0)
                        continue;


                    if (attributeRouteModel == null)
                    {
                        var actionName = action.ActionMethod.Name.ToLower();
                        // 去掉 "Async" 后缀并将方法名称转换为小写
                        if (actionName.EndsWith("async"))
                            actionName = actionName.Substring(0, actionName.Length - "Async".Length);

                        // 动态设置路由规则
                        var routeAttribute = new Microsoft.AspNetCore.Mvc.RouteAttribute($"{actionName}");
                        selector!.AttributeRouteModel = new AttributeRouteModel(routeAttribute);
                        Console.WriteLine($"注册路由: {actionName}");
                    }

                    if(actionConstraints?.Count == 0)
                    {
                        var httpMethod = GetHttpMethod(action.ActionMethod.Name);
                        selector!.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
                        Console.WriteLine($"注册请求方法: {httpMethod}");
                    }

                    // 确保选择器被添加到操作中
                    if (!action.Selectors.Contains(selector))
                        action.Selectors.Add(selector);

                }
            }
        }

        /// <summary>
        /// 根据操作名称动态获取对应的 HTTP 方法
        /// </summary>
        /// <param name="actionName">操作名称</param>
        /// <returns>HTTP 方法</returns>
        private string GetHttpMethod(string actionName)
        {
            if (actionName.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
            {
                return "GET";
            }
            else if (actionName.StartsWith("Add", StringComparison.OrdinalIgnoreCase))
            {
                return "POST";
            }
            else if (actionName.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
            {
                return "PUT";
            }
            else if (actionName.StartsWith("Modify", StringComparison.OrdinalIgnoreCase))
            {
                return "PUT";
            }
            else if (actionName.StartsWith("Edit", StringComparison.OrdinalIgnoreCase))
            {
                return "PUT";
            }
            else if (actionName.StartsWith("Delete", StringComparison.OrdinalIgnoreCase))
            {
                return "DELETE";
            }
            else if (actionName.StartsWith("Remove", StringComparison.OrdinalIgnoreCase))
            {
                return "DELETE";
            }
            // 默认使用 GET 方法
            return "POST";
        }
    }
}
