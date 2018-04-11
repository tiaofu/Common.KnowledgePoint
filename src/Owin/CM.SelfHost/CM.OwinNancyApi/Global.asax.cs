using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;

namespace CM.OwinNancyApi
{
    public class NancyWebApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //这里采用Nancy的方式进行加载
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
