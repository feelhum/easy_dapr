using EasyDapr.Core.Abstractions;
using UserService.Dtos;

namespace UserService.Apps
{
    public interface IUserInfoAppService :IService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserOutputDto> GetUserInfoAsync(int userId);
    }
}
