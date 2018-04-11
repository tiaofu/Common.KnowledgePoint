/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2018-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 测试各种web api的提交方式
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//引入命名空间
using System.Web.Http;

namespace WebApiSubmit
{
    public class WebAPIConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //注册路由映射
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );           
        }
    }
}