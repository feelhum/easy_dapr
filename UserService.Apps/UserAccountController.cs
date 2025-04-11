

using Microsoft.AspNetCore.Mvc;

namespace UserService.Apps
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<bool> LoginAsync(string userNmae, string password)
        {
           return await Task.FromResult(true);
        }
    }
}
