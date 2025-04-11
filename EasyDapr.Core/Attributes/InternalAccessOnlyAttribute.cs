using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InternalAccessOnlyAttribute : Attribute
    {
        // 可选：可以扩展为允许特定的服务
    }
}
