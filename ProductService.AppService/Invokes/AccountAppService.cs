using Dapr.Client;
using EasyDapr.Core.Extensions;

namespace Invokes
{
    public static class AccountAppService
    {
        public static async Task<bool> LoginAsync(this DaprClient daprClient, string username, string password)
        {
            string targetAppId = "User";
            string methodName = "api/Account/login";

            return await daprClient.EasyInvokeMethodAsync<object, bool>(HttpMethod.Post, targetAppId, methodName,  new { username, password });
        }

    }
}
