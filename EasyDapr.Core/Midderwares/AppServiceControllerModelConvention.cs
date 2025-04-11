using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EasyDapr.Core.Midderwares
{
    public class AppServiceControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            // 如果类名以 AppService 结尾，设置为控制器
            if (controller.ControllerType.Name.EndsWith("AppService", StringComparison.OrdinalIgnoreCase))
            {
                // 修改控制器名称，去掉 AppService 后缀
                controller.ControllerName = controller.ControllerType.Name.Replace("AppService", string.Empty);
            }
            else
            {
                Console.WriteLine($"注意：控制器{controller.ControllerType.Name}必须以AppService结尾!");
            }
        }
    }
}
