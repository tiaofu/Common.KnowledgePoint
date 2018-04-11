using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Reflection;
using CM.API;
using System.Web.Http.Dispatcher;

namespace CM.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //Assembly.Load("CM.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");    //加载外部程序集
            var config = new HttpSelfHostConfiguration("http://localhost:8083");
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            //config.Services.Replace(typeof(IAssembliesResolver), new UserResolver());
            config.Services.Replace(typeof(IAssembliesResolver), new WebApiResolver());
            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
    public class HomeController : ApiController
    {
        [HttpGet,HttpPost]
        public string PostGetInfo()
        {
            return "API测试地址";
        }
    }
}
