using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyDapr.Core.Root.Models
{

    public class RouteInfo
    {
        public string Route { get; set; }
        public List<string> HttpMethods { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public List<ParameterInfo> Parameters { get; set; }

        public string NameSpace { get; set; }
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
