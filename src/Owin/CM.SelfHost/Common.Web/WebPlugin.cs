using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Common.Work;
using System.ComponentModel.Composition;
using System.Threading;
namespace Common.Web
{
    //[Export(typeof(IPlugin))] 
    public class WebPlugin : IPlugin
    {
        public void Init()
        {
            Thread th = new Thread(Run);
            th.Start();
            //Run();
        }

        public void Run()
        {
            StartOptions option = new StartOptions();
            string url = "http://localhost:8081";
            option.Urls.Add(url);
            option.Urls.Add("http://192.168.7.212:8081");
            using (WebApp.Start<Startup>(option))
            {
                Console.WriteLine("已启动 {0}", url);
                System.Diagnostics.Process.Start(url + "/index.html");
                while (true) { }
            }
        }
    }
}
