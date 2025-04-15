using EasyDapr.Core.Attributes;
using EasyDapr.Core.Exceptions;
using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.IAppService;
using ProtoBuf.Grpc;
using System.ServiceModel;
using static Google.Rpc.Context.AttributeContext.Types;
namespace ProductService.AppService
{
    
    public class ProductAppService : EasyDaprService, IProductAppService
    {
        [InternalAccessOnlyAttribute]
        public async Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input)
        {
            if (input == null)
            {
                Console.WriteLine("Request body is null.");
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            throw new Exception("error");

            //throw new UserFriendlyException("错误");


            var response = await Task.FromResult(new GetProductOutput() { result= $"id is {input.id}" });

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
            Console.WriteLine($"Serialized response: {jsonResponse}");

            return response;
        }
    }
}
