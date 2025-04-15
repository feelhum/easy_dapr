using EasyDapr.Core.Midderwares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.IAppService;

namespace UserService.AppService
{
    public class AccountAppService : EasyDaprService, IAccountAppService
    {
        public async Task<bool> Login(string username, string password)
        {
            if (username == "admin" && password == "123456")
                return await Task.FromResult(true);
            return await Task.FromResult(false);
        }
    }
}
