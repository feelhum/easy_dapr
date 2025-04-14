using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using System.ServiceModel;

namespace ProductService.IAppService
{
    [ServiceContract]
    public interface IProductAppService
    {
        [OperationContract]
        Task<GetProductOutput> GetProductAsync([FromBody] GetProductInput input);
    }
}
