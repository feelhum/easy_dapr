using EasyDapr.Core.Root.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Root
{
    public interface IServiceRoutAppService
    {
        ICollection<RouteInfo> GetRoutes();
    }
}
