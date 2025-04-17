using Dapr.Client;
using EasyDapr.Core.Attributes;
using EasyDapr.Core.Dtos;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Extensions;
using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
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
        //[InternalAccessOnly]
        public async Task<UserOutputDto> GetUserInfoAsync([FromBody] IdInput input)
        {
            var result = await _freeSql.Select<User>().Where(u => u.Id == input.Id).FirstAsync<UserOutputDto>();
            if (result == null)
                throw new UserFriendlyException("不存在的用户");

            try { 
            return result;
            }catch (Exception ex)
            {
                throw new UserFriendlyException("获取用户信息失败"+ex.Message);
            }
            
        }
        [HttpGet("GetUserInfoTest/{id}/x")]
        public async Task<UserOutputDto> GetUserInfoTestAsync(int id)
        {
            var result = await _freeSql.Select<User>().Where(u => u.Id == id).FirstAsync<UserOutputDto>();
            if (result == null)
                throw new UserFriendlyException("不存在的用户");

            try
            {
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("获取用户信息失败" + ex.Message);
            }

        }

        public async Task<GetProductOutput> GetUser(IdInput input)
        {
            // 创建 Dapr 客户端
            using var daprClient = new DaprClientBuilder().Build();

            // 目标服务的 AppId
            string targetAppId = "Product";

            // 调用目标服务的方法
            string methodName = "api/Product/GetProduct"; // 格式：接口名/方法名

            // 请求数据
            var requestData = new GetProductInput { id = 1 };

            // 调用目标服务的 gRPC 方法
            var result = await daprClient.EasyInvokeMethodAsync<GetProductInput, GetProductOutput>(
                httpMethod: HttpMethod.Post,  // HTTP 方法
                targetAppId,          // 目标服务的 AppId 
                methodName,           // 目标服务的方法名
                requestData           // 请求数据
            );
            Console.WriteLine($"Result: {System.Text.Json.JsonSerializer.Serialize(result)}");
            return result;


            //从 Dapr 的状态存储中获取用户信息
            var user = await _daprClient.GetStateAsync<string>("statestore", $"user-{input.Id}");
            return new GetProductOutput() { result = "产品资料" };
        }

        public async Task<UserOutputDto> AddUser([FromBody] UserInputDto input)
        {
            // 将用户信息保存到 Dapr 的状态存储
            await _daprClient.SaveStateAsync("statestore", $"user-{input.Name}", input.Name);
            return new UserOutputDto() { Email =input.Email, Password = input.Password, Name =input.Name,Id=2 };
        }


        public async Task<bool> TestUserAsync(string f1,bool f2,long f3,int f4)
        {
            // 将用户信息保存到 Dapr 的状态存储
            await _daprClient.SaveStateAsync("statestore", $"user-{111}", "awd");
            return true;
        }

    }
}
