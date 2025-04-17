using EasyDapr.Core.Dtos;
using ProductService.Dtos;
using UserService.Dtos;

namespace UserService.IAppService
{
    public interface IUserAppService
    {

       Task<UserOutputDto> GetUserInfoAsync(IdInput input);

        Task<UserOutputDto> GetUserInfoTestAsync(int id);

        Task<GetProductOutput> GetUser(IdInput input);

        Task<UserOutputDto> AddUser(UserInputDto input);

        Task<bool> TestUserAsync(string f1, bool f2, long f3, int f4);
    }
}
