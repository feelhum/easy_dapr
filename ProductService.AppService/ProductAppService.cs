using EasyDapr.Core.Attributes;
using EasyDapr.Core.Dtos;
using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.IAppService;
using Invokes;
using Dapr.Client;
using EasyDapr.Core.Exceptions;
namespace ProductService.AppService
{

    public class ProductAppService : EasyDaprService, IProductAppService
    {
        [InternalAccessOnly]
        [HttpPost]
        public async Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input)
        {
            if (input == null)
            {
                Console.WriteLine("Request body is null.");
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            //throw new Exception("系统错误，请联系管理员");

            throw new UserFriendlyException("系统错误，请联系管理员");


            var response = await Task.FromResult(new GetProductOutput() { result= $"id is {input.id}" });

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
