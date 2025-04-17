using Dapr.Client;
using EasyDapr.Core.Dtos;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using Grpc.Net.Client.Configuration;
using Invokes;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.IAppService;
using UserService.Dtos;
using static Google.Rpc.Context.AttributeContext.Types;

namespace ProductService.AppService
{

    public class ProductAppService : EasyDaprService, IProductAppService
    {
        private readonly DaprClient _daprClient;

        public ProductAppService(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }
        //[InternalAccessOnly]
        [HttpPost]
        public async Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input)
        {
            if (input == null)
            {
                Console.WriteLine("Request body is null.");
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }
            string targetAppId = "User";
            string methodName = "api/User/getuserinfotest/1/x";
            var result = await _daprClient.InvokeMethodAsync<UserOutputDto>(
                    httpMethod: HttpMethod.Get,  // HTTP 方法
                    targetAppId,          // 目标服务的 AppId
                    methodName           // 目标服务的方法名
                );
            var x = await _daprClient.AddUserAsync(new UserInputDto() { Email="111@qq.com",  Name="111", Password="111"});

            //var y = await _daprClient.GetUserInfoTestAsyncAsync(1);

            // throw new UserFriendlyException($"系统错误，请联系管理员{x.ToString()}");


            var response = await Task.FromResult(new GetProductOutput() { result= $"id is {input.id},{x.Email},{x.Name};;;,,,result is {result.Name}" });

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
            Console.WriteLine($"Serialized response: {jsonResponse}");

            return response;
        }

        public async Task<string> GetUserAsync(int id)
        {
            using var daprClient = new DaprClientBuilder().Build();
            return (await daprClient.GetUserAsync(new IdInput { Id = id })).result;
        }
    }
}
