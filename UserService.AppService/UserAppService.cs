using Dapr.Client;
using EasyDapr.Core.Attributes;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using UserService.Dtos;
using UserService.Entities;
using UserService.IAppService;

namespace UserService.AppService
{
    public class UserAppService : EasyDaprService, IUserAppService
    {
        private readonly IFreeSql _freeSql;
        private readonly DaprClient _daprClient;
        public UserAppService(IFreeSql freeSql, DaprClient daprClient)
        {
            _freeSql = freeSql;
            _daprClient = daprClient;

        }
        [HttpPost]
        [InternalAccessOnly]
        public async Task<UserOutputDto> GetUserInfoAsync(int userId)
        {
            var result = await _freeSql.Select<User>().Where(u => u.Id == userId).FirstAsync<UserOutputDto>();
            if (result == null)
                throw new UserFriendlyException("不存在的用户");
            try { 
            return result;
            }catch (Exception ex)
            {
                throw new UserFriendlyException("获取用户信息失败"+ex.Message);
            }
            
        }

        public async Task<string> GetUser(int id)
        {
            try
            {
                // 创建 Dapr 客户端
                using var client = new DaprClientBuilder().Build();

                // 目标服务的 AppId
                string targetAppId = "productapp";

                // 调用目标服务的方法
                string methodName = "api/product/GetProduct"; // 目标服务中暴露的方法名

                // 请求数据（如果需要发送请求体）
                var requestData = new { Id = 1 };

                // 通过 Dapr 调用目标服务的 HTTP 方法
                var result = await client.InvokeMethodAsync<object, string>(
                    targetAppId,          // 目标服务的 AppId
                    methodName,           // 目标服务的方法名
                    requestData           // 请求数据
                );

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            // 从 Dapr 的状态存储中获取用户信息
            var user = await _daprClient.GetStateAsync<string>("statestore", $"user-{id}");
            return user ?? $"User {id} not found";
        }

        public async Task<bool> AddUser(int id, string name)
        {
            // 将用户信息保存到 Dapr 的状态存储
            await _daprClient.SaveStateAsync("statestore", $"user-{id}", name);
            return true;
        }
    }
}
