using ProductService.Dtos;

namespace ProductService.IAppService
{
    public interface IProductAppService
    {

        Task<string> GetProductAsync(GetProductInput input);
    }
}
