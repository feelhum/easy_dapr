using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.IAppService;
using ProtoBuf.Grpc;
using System.ServiceModel;
namespace ProductService.AppService
{
    
    public class ProductAppService : EasyDaprService, IProductAppService
    {
        public async Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input)
        {
            if (input == null)
            {
                Console.WriteLine("Request body is null.");
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            Console.WriteLine($"Received input: {input.id}");
            return await Task.FromResult(new GetProductOutput() { result= $"产品id是：{input.id}" });
        }
    }
}
