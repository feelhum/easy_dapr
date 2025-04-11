using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;
using FreeSql;
using UserService.Entities;
using EasyDapr.Core.HttpMidderware;
using EasyDapr.Core.Filters;
namespace UserService.Apps
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
    {

        private readonly IFreeSql _freeSql;
        public UserInfoController(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        [HttpGet("GetUserInfo")]
        [HttpTimeout(1000)]
        public async Task<UserOutputDto> GetUserInfoAsync(int userId)
        {
            await Task.Delay(3000); // 模拟慢接口
            var result = await _freeSql.Select<User>().Where(u => u.Id == userId).FirstAsync<UserOutputDto>();
            if (result == null)
                throw new BizException("用户不存在");
            return result;
        }
    }
}
