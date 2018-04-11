using Nancy.Hosting.Self;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using Owin;
using Nancy;

namespace CM.OwinNancyApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }

        private static NancyHost _host = null;
        /// <summary>
        /// 监听端口 启动站点
        /// </summary>
        /// <param name="urls">监听ip端口列表</param>
        public static NancyHost Start(int port)
        {
            try
            {
                //AreaRegistration.RegisterAllAreas();
                //GlobalConfiguration.Configure(WebApiConfig.Register);
                _host = new NancyHost(new Uri(string.Format("http://localhost:{0}", port)));
                _host.Start();              
                return _host;
            }
            catch (HttpListenerException ex)
            {                
                Random random = new Random();
                port = random.Next(port - 1000, port + 1000);
                Console.WriteLine(" 重新尝试端口:" + port);
                return Start(port);
            }
            catch (Exception ex)
            {               
                throw ex;
            }
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public static void Dispose()
        {
            //回收资源
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
            }
        }
    }
}
