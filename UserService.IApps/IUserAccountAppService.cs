using EasyDapr.Core.Abstractions;

namespace UserService.IApps
{
    public interface IUserAccountAppService : IService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userNmae"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> LoginAsync(string userNmae, string password);
    }
}
