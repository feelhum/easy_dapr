using System.Net.Http;
using Dapr.Client;
using UserService.Dtos;
using EasyDapr.Core.Dtos;
using EasyDapr.Core.Extensions;
using ProductService.Dtos;

namespace Invokes
{
    public static class UserAppService
    {
        public static async Task<UserOutputDto> GetUserInfoAsyncAsync(this DaprClient daprClient, IdInput input)
        {
            string targetAppId = "User";
            string methodName = "api/User/getuserinfo";

            return await daprClient.EasyInvokeMethodAsync<IdInput, UserOutputDto>(HttpMethod.Post, targetAppId, methodName,  input);
        }

        public static async Task<GetProductOutput> GetUserAsync(this DaprClient daprClient, IdInput input)
        {
            string targetAppId = "User";
            string methodName = "api/User/getuser";

            return await daprClient.EasyInvokeMethodAsync<IdInput, GetProductOutput>(HttpMethod.Get, targetAppId, methodName,  input);
        }

        public static async Task<bool> AddUserAsync(this DaprClient daprClient, UserInputDto input)
        {
            string targetAppId = "User";
            string methodName = "api/User/adduser";

            return await daprClient.EasyInvokeMethodAsync<UserInputDto, bool>(HttpMethod.Post, targetAppId, methodName,  input);
        }

        public static async Task<bool> TestUserAsyncAsync(this DaprClient daprClient, string f1, bool f2, long f3, int f4)
        {
            string targetAppId = "User";
            string methodName = "api/User/testuser";

            return await daprClient.EasyInvokeMethodAsync<object, bool>(HttpMethod.Post, targetAppId, methodName,  new { f1, f2, f3, f4 });
        }

    }
}
