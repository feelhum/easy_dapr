using EasyDapr.Core.Midderwares;
using UserService.Dtos;

namespace UserService.IAppService
{
    public interface IUserAppService
    {

       Task<UserOutputDto> GetUserInfoAsync(int userId);

        Task<string> GetUser(int id);

        Task<bool> AddUser(int id, string name);
    }
}
