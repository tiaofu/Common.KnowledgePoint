using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using System.Diagnostics;
using Microsoft.Owin.Hosting;

namespace CM.OwinNancyApi
{
    class Program
    {
        static void Main(string[] args)
        {
            //1.自启动方式
            //using (NancyHost host = Startup.Start(9000))
            //{
            //    //调用系统默认的浏览器   
            //    string url = string.Format("http://127.0.0.1:{0}", 9000);
            //    Process.Start(url);
            //}
            var url = "http://localhost:8080";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
