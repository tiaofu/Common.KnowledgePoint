using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Task;
using Nancy.Hosting.Self;
using System.ComponentModel.Composition;
namespace Web.Task
{
    [Export(typeof(IWork))]
    public class WebWorkRegist : IWork
    {
        public void Regist()
        {
            try
            {
                int port = 9000;
                string url = string.Format("http://localhost:{0}", port);
                var _host = new NancyHost(new Uri(url));
                _host.Start();
                //Process.Start(url);
                Console.WriteLine("站点启动成功,请打开{0}进行浏览", url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("站点启动失败：" + ex.Message);
            }
            Console.ReadKey();
        }
    }
}
