using EasyDapr.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using System.ServiceModel;

namespace ProductService.IAppService
{
    public interface IProductAppService
    {
        Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> GetUserAsync(int  id);
    }
}
