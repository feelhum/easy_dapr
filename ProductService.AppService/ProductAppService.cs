using EasyDapr.Core.Midderwares;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.IAppService;

namespace ProductService.AppService
{
    [ApiController]
    [Route("api/product")]
    public class ProductAppService : EasyDaprService, IProductAppService
    {
        [HttpPost("GetProduct")]
        public async Task<string> GetProductAsync([FromBody] GetProductInput input)
        {
            Console.WriteLine($"Received input: {input?.id}");
            return await Task.FromResult($"产品id是：{input.id}");
        }
    }
}
