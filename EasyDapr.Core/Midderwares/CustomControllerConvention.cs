using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EasyDapr.Core.Midderwares
{
    public class CustomControllerConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            // 动态生成路由规则
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel == null) // 如果没有显式声明 [Route]
                {
                    var controllerName = controller.ControllerType.Name;

                    // 去掉类名中的 "AppService" 后缀
                    if (controllerName.EndsWith("AppService"))
                    {
                        controllerName = controllerName.Substring(0, controllerName.Length - "AppService".Length);
                        // 设置默认路由规则: api/{ControllerName}
                        selector.AttributeRouteModel = new AttributeRouteModel(
                            new Microsoft.AspNetCore.Mvc.RouteAttribute($"api/{controllerName}")
                        );
                    }
                }
            }
        }
    }
}
